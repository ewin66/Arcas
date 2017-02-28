using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Arcas;
using Cav;
using Cav.BaseClases;
using Arcas.BL.TFS;
using Arcas.DBA;

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

        private String beginTranText = Environment.NewLine + @"BEGIN TRY
BEGIN TRAN" + Environment.NewLine;
        private String closeTranText = Environment.NewLine + @"COMMIT TRAN
END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();
            
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState );

ROLLBACK TRAN
END CATCH";

        private String verOnDB = @"IF EXISTS(
    SELECT
        *
    FROM 
        sys.extended_properties
    WHERE extended_properties.class_desc = 'DATABASE' AND extended_properties.NAME = 'Version'
    )
    EXEC sys.sp_dropextendedproperty @name=N'Version'

EXEC sys.sp_addextendedproperty @name=N'Version', @value=N'{0}'" + Environment.NewLine;

        private ArcasDBAdapter adapter = new ArcasDBAdapter();


        /// <summary>
        /// Накатить скрипт
        /// </summary>
        /// <param name="TBlink">Настройка связки TFS-DB</param>
        /// <param name="SqlScript">Тело скрипта</param>
        /// <param name="Comment">Комментарий к заливке</param>
        /// <returns></returns>
        public String SaveScript(TfsDbLink TBlink, String SqlScript, String Comment, Boolean InTaransaction)
        {

            if (Comment.IsNullOrWhiteSpace())
                return "Необходимо заполнить комментрарий";

            if (SqlScript.IsNullOrWhiteSpace())
                return "Тело скрипта пустое";


            if (checkSqlScriptOnUSE(SqlScript))
                return "В скрипте используется USE БД.";

            SqlScript = SqlScript.Trim();

            using (TFSRoutineBL tfsbl = new TFSRoutineBL())
            {
                // Проверяем переданные соединения с TFS и БД
                try
                {
                    SendStat("Подключаемся к БД");
                    // Инициализируем соединение с БД
                    DomainContext.InitConnection(TBlink.DB.ConnectionString);

                    // Проверяем настройки TFS
                    SendStat("Подключаемся к TFS");
                    tfsbl.VersionControl(TBlink.TFS.Server);
                    tfsbl.MapTempWorkspace(TBlink.TFS.Path);
                }
                catch (Exception ex)
                {
                    return ex.Expand();
                }

                // TODO  Проверить скрип на корректность 
                // В частности, отсутствие USE. Ещеб как нить замутить просто проверку, а не выполнение

                SendStat("Обработка файла версионности");

                String VerFileName = "LastVer.xml";
                String PathVerFile = Path.Combine(tfsbl.Tempdir, VerFileName);

                if (tfsbl.GetLastFile(VerFileName) == 0)
                {
                    //файла нет
                    if (!File.Exists(PathVerFile))
                    {
                        // сохраняем новую чистую версию ДБ
                        (new VerDB()).XMLSerialize(PathVerFile);
                        tfsbl.AddFile(PathVerFile);
                        tfsbl.CheckIn("Добавлен файл версионности");
                    }
                }

                if (!tfsbl.LockFile(VerFileName))
                    return "Производится накатка. Повторите позже";

                if (!tfsbl.CheckOut(PathVerFile))
                    return "Накатка неуспешна. Повторите позже";

                var CurVerDB = PathVerFile.XMLDeserializeFromFile<VerDB>() ?? new VerDB();

                CurVerDB.VersionBD += 1;
                CurVerDB.DateVersion = new DateTimeOffset(DateTime.Now).DateTime;

                var Scts = SplitSqlTExtOnGO(SqlScript);

                SendStat("Накатка скрипта на БД");

                DbTransactionScope tran = null;

                try
                {
                    if (InTaransaction)
                        tran = new DbTransactionScope();

                    foreach (var sct in Scts)
                        adapter.ExecScript(sct);

                    adapter.ExecScript(String.Format(verOnDB, CurVerDB));

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
                // Накатываем скрипт. В транзанкции. Надо уточнить, может 2 раза накатывать, что б

                // типа все норм. Сохраняем

                // комментарий к скрипту
                String commentOnScript = String.Format(@"--{0} Комп:{1} Пользователь:{2}\{3}{4}",
                    CurVerDB,
                    Environment.MachineName,
                    Environment.UserDomainName,
                    Environment.UserName,
                    Environment.NewLine + "--" + Comment.Replace(Environment.NewLine, Environment.NewLine + "-- ") + Environment.NewLine
                    );

                String FileNameNewVer = Path.Combine(tfsbl.Tempdir, CurVerDB.ToString() + ".sql");

                File.AppendAllText(FileNameNewVer, commentOnScript);

                if (InTaransaction)
                    File.AppendAllText(FileNameNewVer, beginTranText);

                foreach (var sct in Scts)
                    File.AppendAllText(FileNameNewVer, "EXEC('" + sct.Replace("'", "''") + "')" + Environment.NewLine);

                File.AppendAllText(FileNameNewVer, "EXEC('" + String.Format(verOnDB, CurVerDB).Replace("'", "''") + "')" + Environment.NewLine);

                if (InTaransaction)
                    File.AppendAllText(FileNameNewVer, closeTranText);

                CurVerDB.XMLSerialize(PathVerFile);

                SendStat("Чекин в TFS");

                tfsbl.AddFile(FileNameNewVer);
                tfsbl.CheckIn(Comment);

                SendStat("Готово");

                return null;
            }
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
    }
}
