using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Arcas.BL.TFS;
using Arcas.Settings;
using Cav;
using Cav.DataAcces;

namespace Arcas.BL
{
    public delegate void ProgressStateDelegat(String Message);

    public class TfsDBSaveBL
    {
        public event ProgressStateDelegat StatusMessages;
        private void SendStat(String mess)
        {
            if (StatusMessages != null)
                StatusMessages(mess);
        }

        Adapter adapter = new Adapter();

        /// <summary>
        /// Накатить скрипт
        /// </summary>
        /// <param name="TBlink">Настройка связки TFS-DB</param>
        /// <param name="SqlScript">Тело скрипта</param>
        /// <param name="Comment">Комментарий к заливке</param>
        /// <returns></returns>        
        public String SaveScript(
            TfsDbLink TBlink,
            String SqlScript,
            String Comment,
            Boolean InTaransaction,
            List<int> linkedTask)
        {

            if (Comment.IsNullOrWhiteSpace())
                return "Необходимо заполнить комментрарий";

            if (SqlScript.IsNullOrWhiteSpace())
                return "Тело скрипта пустое";


            if (checkSqlScriptOnUSE(SqlScript))
                return "В скрипте используется USE БД.";


            Func<String, String> trimLines = (a) =>
            {
                a = a.Trim();

                var colStr = a.Replace(Environment.NewLine, new String('\r', 1)).Split(new char[] { '\r' });

                for (int i = 0; i < colStr.Length; i++)
                    colStr[i] = colStr[i].TrimEnd();

                return colStr.JoinValuesToString(Environment.NewLine, false);
            };

            // причесываем текстовки
            SqlScript = trimLines(SqlScript);

            Comment = trimLines(Comment);
            Comment = "--" + Comment.Replace(Environment.NewLine, Environment.NewLine + "-- ") + Environment.NewLine;

            UpdateDbSetting upsets = null;
            bool useSqlConnection = false;

            using (TFSRoutineBL tfsbl = new TFSRoutineBL())
            {

                // Проверяем переданные соединения с TFS и БД
                try
                {

                    // Проверяем настройки TFS
                    SendStat("Подключаемся к TFS");
                    tfsbl.VersionControl(TBlink.ServerUri);

                    SendStat("Получение настроек поднятия версии.");
                    var tempfile = Path.Combine(DomainContext.TempPath, Guid.NewGuid().ToString());
                    tfsbl.DownloadFile(TBlink.ServerPathToSettings, tempfile);

                    try
                    {
                        upsets = File.ReadAllBytes(tempfile).DeserializeAesDecrypt<UpdateDbSetting>(TBlink.ServerPathToSettings);
                    }
                    catch (Exception ex)
                    {
                        return "Получение файла настроек неуспешно. Exception: " + ex.Expand();
                    }

                    if (upsets == null || upsets.ServerPathScripts.IsNullOrWhiteSpace())
                    {
                        return "Получение файла настроек неуспешно";
                    }

                    if (upsets.AssemplyWithImplementDbConnection != null)
                        upsets.AssemplyWithImplementDbConnection = upsets.AssemplyWithImplementDbConnection.GZipDecompress();


                    SendStat("Получение типа соединения");

                    Type conn = typeof(SqlConnection);
                    useSqlConnection = true;

                    if (upsets.TypeConnectionFullName != conn.ToString())
                    {
                        if (upsets.AssemplyWithImplementDbConnection == null)
                            return $"В настроках указан тип DbConnection '{upsets.TypeConnectionFullName}', но отсутствует бинарное представление сборки реализации";

                        AppDomain.CurrentDomain.Load(upsets.AssemplyWithImplementDbConnection);

                        conn = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.ExportedTypes).FirstOrDefault(x => x.FullName == upsets.TypeConnectionFullName);

                        if (conn == null)
                            return $"Не удалось найти тип DbConnection '{upsets.TypeConnectionFullName}'";
                        useSqlConnection = false;
                    }

                    SendStat("Подключаемся к БД");
                    DomainContext.InitConnection(conn, upsets.ConnectionStringModelDb);


                    tfsbl.MapTempWorkspace(upsets.ServerPathScripts);

                }
                catch (Exception ex)
                {
                    return ex.Expand();
                }

                // TODO  Проверить скрип на корректность 
                // В частности, отсутствие USE. Ещеб как нить замутить просто проверку, а не выполнение

                SendStat("Обработка файла версионности");

                String VerFileName = ".lastVer.xml";
                String PathVerFile = Path.Combine(tfsbl.Tempdir, VerFileName);

                if (tfsbl.GetLastFile(VerFileName) == 0)
                {
                    //файла нет
                    if (!File.Exists(PathVerFile))
                    {
                        // сохраняем новую чистую версию ДБ
                        (new VerDB()).XMLSerialize(PathVerFile);
                        tfsbl.AddFile(PathVerFile);
                        tfsbl.CheckIn("Добавлен файл версионности", linkedTask);
                    }
                }

                if (!tfsbl.LockFile(VerFileName))
                    return "Производится накатка. Повторите позже";

                if (!tfsbl.CheckOut(PathVerFile))
                    return "Извлечение файла текущей версии неуспешно. Повторите позже";

                var CurVerDB = PathVerFile.XMLDeserializeFromFile<VerDB>() ?? new VerDB();

                CurVerDB.VersionBD += 1;
                CurVerDB.DateVersion = new DateTimeOffset(DateTime.Now).DateTime;

                List<String> scts = new List<string>();

                if (useSqlConnection)
                    scts.AddRange(SplitSqlTExtOnGO(SqlScript));
                else
                    scts.Add(SqlScript);

                SendStat("Накатка скрипта на БД");

                DbTransactionScope tran = null;

                try
                {
                    if (InTaransaction)
                        tran = new DbTransactionScope();

                    foreach (var sct in scts)
                        adapter.ExecScript(sct);

                    adapter.ExecScript(String.Format(upsets.ScriptUpdateVer, CurVerDB));

                    if (tran != null)
                        tran.Complete();
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        ((IDisposable)tran).Dispose();
                    return ex.Expand();
                }


                SendStat("Генерация файла скрипта");

                var sb = new StringBuilder();

                sb.AppendLine(Comment);

                if (InTaransaction)
                    sb.AppendLine(upsets.ScriptPartBeforeBodyWithTran);

                foreach (var item in scts)
                {
                    var script = item;
                    if (useSqlConnection)
                        script = "EXEC('" + script.Replace("'", "''") + "')";

                    sb.AppendLine(script);
                }

                sb.AppendLine(String.Format(upsets.ScriptUpdateVer, CurVerDB));

                if (InTaransaction)
                    sb.Append(upsets.ScriptPartBeforeBodyWithTran);

                String FileNameNewVer = Path.Combine(tfsbl.Tempdir, CurVerDB + ".sql");

                File.WriteAllText(FileNameNewVer, sb.ToString());
                CurVerDB.XMLSerialize(PathVerFile);

                SendStat("Чекин в TFS");

                tfsbl.AddFile(FileNameNewVer);
                tfsbl.CheckIn(Comment, linkedTask);

                SendStat("Готово");
            }

            return null;
        }

        /// <summary>
        /// Проверка на наличие в скрипте USE
        /// </summary>
        /// <param name="SqlText">SQL скрипт</param>
        /// <returns>true - USE в скрипте</returns>
        private Boolean checkSqlScriptOnUSE(String SqlText)
        {
            // убираем строчные комментарии
            Regex regex = new Regex("--.*", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            SqlText = regex.Replace(SqlText, Environment.NewLine);

            // убираем многострочные коммантарии
            regex = new Regex(@"/\*.*?(\*/)+", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            SqlText = regex.Replace(SqlText, "");

            // убираем [*use*](имена таблиц или полей. заключены в квадратные скобки)
            regex = new Regex(@"\[[^\[]*use[^\[\]]*]", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            SqlText = regex.Replace(SqlText, "");

            // ищем use БД
            regex = new Regex(@"([^\S]|^)use\b", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return regex.Match(SqlText).Success;
        }

        /// <summary>
        /// Разбиение скрипта по GO
        /// </summary>
        /// <param name="SqlText"></param>
        /// <returns></returns>
        private List<String> SplitSqlTExtOnGO(String SqlText)
        {
            string separator = @"!@#$%^&*()";
            // убираем строчные комментарии
            Regex regex = new Regex(@"(\n|\r|\n\r|^)\s*GO\s*(\n\r|\n|\r|$)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
            SqlText = regex.Replace(SqlText, separator);

            var res = new List<String>(SqlText.Split(new string[] { separator }, StringSplitOptions.None));

            res.RemoveAll(new Predicate<string>((a) => { return a == String.Empty; }));

            return res;
        }

        private class Adapter : DataAccesBase
        {
            public void ExecScript(String SqlText)
            {
                var cmd = this.CreateCommandObject();
                cmd.CommandText = SqlText;
                ExecuteNonQuery(cmd);
            }
        }
    }
}
