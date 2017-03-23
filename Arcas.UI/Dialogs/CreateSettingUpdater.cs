using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Arcas.BL.TFS;
using Cav;
using Cav.Tfs;
using Cav.WinForms.BaseClases;

namespace Arcas.Settings
{
    public partial class CreateSettingUpdater : DialogFormBase
    {
        private class ErrorTracker
        {
            private HashSet<Control> mErrors = new HashSet<Control>();
            private ErrorProvider mProvider;

            public ErrorTracker(ErrorProvider provider)
            {
                mProvider = provider;
            }
            public void SetError(Control ctl, string text)
            {
                if (string.IsNullOrEmpty(text))
                    mErrors.Remove(ctl);
                else
                    if (!mErrors.Contains(ctl))
                    mErrors.Add(ctl);
                mProvider.SetError(ctl, text);
            }
            public int Count { get { return mErrors.Count; } }

            public override string ToString()
            {
                if (!mErrors.Any())
                    return null;

                return mErrors.Select(x => mProvider.GetError(x)).JoinValuesToString(Environment.NewLine);
            }
        }

        private class DbTypeItem
        {
            public Type ConType { get; set; }
            public byte[] AssembyFile { get; set; }
            public override string ToString()
            {
                if (ConType == null)
                    return "<Добавить сборку>";
                return ConType.ToString();
            }
        }

        public CreateSettingUpdater()
        {
            InitializeComponent();

            errorTracker = new ErrorTracker(errorProvider);

            cmbDbConectionType.Items.Add(new DbTypeItem() { ConType = typeof(SqlConnection) });
            cmbDbConectionType.Items.Add(new DbTypeItem());
            cmbDbConectionType.SelectedIndex = 0;

            errorTracker.SetError(tbSettingName, "Не указано наименование связки");
            errorTracker.SetError(tbTfsProject, "Не указан проект TFS");
            errorTracker.SetError(tbSetFileName, "Не указано наименование файла настроек");
            errorTracker.SetError(tbSetFileServerFolder, "Не указан путь сохранения файла настроек на сервере");
            errorTracker.SetError(tbFolderForScripts, "Не указан путь сохранения скриптов на сервере");
            errorTracker.SetError(tbConnectionString, "Не указана строка соединения с модельной БД");
        }


        private ErrorTracker errorTracker = null;
        private WrapTfs wrapTfs = new WrapTfs();
        public TFSDBList ItemsInSets { get; set; }

        private void tbSettingName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tbSettingName.Text.IsNullOrWhiteSpace())
            {
                errorTracker.SetError(tbSettingName, "Не указано наименование связки");
                return;
            }

            if (ItemsInSets.Any(x => x.Name == tbSettingName.Text))
            {
                errorTracker.SetError(tbSettingName, "Наименование должно быть уникальным");
                return;
            }

            errorTracker.SetError(tbSettingName, "");
        }
        private void tbTfsProject_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tbTfsProject.Text.IsNullOrWhiteSpace())
            {
                errorTracker.SetError(tbTfsProject, "Не указан проект TFS");
                return;
            }

            errorTracker.SetError(tbTfsProject, "");
            btSetPathFolderSetFile.Enabled = !tbTfsProject.Text.IsNullOrWhiteSpace();
            btScriptFolder.Enabled = !tbTfsProject.Text.IsNullOrWhiteSpace();

        }
        private void button2_Click(object sender, EventArgs e)
        {
            var serverProj = wrapTfs.ShowTeamProjectPicker(this);
            if (serverProj == null)
                return;
            tbTfsProject.Text = serverProj.ToString();
            tbTfsProject_Validating(null, null);
        }
        private void tbSetFileName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tbSetFileName.Text.IsNullOrWhiteSpace())
                errorTracker.SetError(tbSetFileName, "Не указано наименование файла настроек");
            else
                errorTracker.SetError(tbSetFileName, null);


            tbSetFileName.Text = tbSetFileName.Text.ReplaceInvalidPathChars();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var vc = wrapTfs.VersionControlServerGet(new Uri(tbTfsProject.Text));
            var folder = wrapTfs.ShowDialogChooseServerFolder(this, vc, null);
            tbSetFileServerFolder.Text = folder;
            tbSetFileServerFolder_Validating(null, null);
        }
        private void tbSetFileServerFolder_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tbSetFileServerFolder.Text.IsNullOrWhiteSpace())
                errorTracker.SetError(tbSetFileServerFolder, "Не указан путь сохранения файла настроек на сервере");
            else
                errorTracker.SetError(tbSetFileServerFolder, null);
        }
        private void cmbDbConectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDbConectionType.SelectedIndex < 0)
                return;

            DbTypeItem selItem = (DbTypeItem)cmbDbConectionType.SelectedItem;

            if (selItem.ConType != null)
            {
                tbConnectionString_Validating(null, null);
                return;
            }

            var filePathAssembly = Dialogs.FileBrowser(
                    Owner: this,
                    Title: "Выбор сборки с реализацией DbConnection",
                    DefaultExt: ".ddl",
                    Filter: "Assemblys dll|*.dll",
                    AddExtension: false).FirstOrDefault();

            if (filePathAssembly.IsNullOrWhiteSpace())
            {
                cmbDbConectionType.SelectedIndex = 0;
                return;
            }

            Assembly fileAssembly = null;
            byte[] fileAssemblyRaw = null;

            try
            {
                fileAssembly = Assembly.LoadFile(filePathAssembly);
                fileAssemblyRaw = File.ReadAllBytes(filePathAssembly);
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
                cmbDbConectionType.SelectedIndex = 0;
                return;
            }

            foreach (var asType in fileAssembly.ExportedTypes.Where(x => x.IsSubclassOf(typeof(DbConnection))))
            {
                var item = new DbTypeItem();
                item.ConType = asType;
                item.AssembyFile = fileAssemblyRaw;
                cmbDbConectionType.Items.Insert(cmbDbConectionType.Items.Count - 1, item);
                cmbDbConectionType.SelectedItem = item;
            }

            if (cmbDbConectionType.SelectedItem == selItem)
                cmbDbConectionType.SelectedIndex = 0;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var vc = wrapTfs.VersionControlServerGet(new Uri(tbTfsProject.Text));
            var folder = wrapTfs.ShowDialogChooseServerFolder(this, vc, null);
            tbFolderForScripts.Text = folder;
            tbFolderForScripts_Validating(null, null);

        }
        private void tbFolderForScripts_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tbFolderForScripts.Text.IsNullOrWhiteSpace())
                errorTracker.SetError(tbFolderForScripts, "Не указан путь сохранения скриптов на сервере");
            else
                errorTracker.SetError(tbFolderForScripts, null);
        }
        private void tbConnectionString_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tbConnectionString.Text.IsNullOrWhiteSpace())
                errorTracker.SetError(tbConnectionString, "Не указана строка соединения с модельной БД");
            else
                errorTracker.SetError(tbConnectionString, null);

            btChekConnection.Enabled = !tbConnectionString.Text.IsNullOrWhiteSpace();
            if (btChekConnection.Enabled)
                errorTracker.SetError(btChekConnection, "Необходимо проверить строку соединения");
        }
        private void btChekConnection_Click(object sender, EventArgs e)
        {
            DbTypeItem selitem = (DbTypeItem)cmbDbConectionType.SelectedItem;

            String context = tbConnectionString.Text;

            try
            {
                DomainContext.InitConnection(selitem.ConType, context);
                Dialogs.InformationF(this, "Тест соединения успешен.");
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
                errorTracker.SetError(btChekConnection, "Проверка строки соединения неуспешна");
                return;
            }

            errorTracker.SetError(btChekConnection, null);
        }

        private void tbNumberTask_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.KeyChar);
        }

        private void CreateSettingUpdater_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.UserClosing & e.CloseReason != CloseReason.None) ||
                this.DialogResult != DialogResult.OK)
                return;

            if (errorTracker.Count != 0)
            {
                Dialogs.ErrorF(this, errorTracker.ToString());
                e.Cancel = true;
                return;
            }

            DbTypeItem dbItem = (DbTypeItem)cmbDbConectionType.SelectedItem;

            int? taskId = null;
            int tid;
            if (int.TryParse(tbNumberTask.Text, out tid))
                taskId = tid;

            // формируем настройку для получения настроек
            TfsDbLink newLink = new TfsDbLink();
            newLink.Name = tbSettingName.Text;
            newLink.ServerUri = new Uri(tbTfsProject.Text);
            newLink.ServerPathToSettings = tbSetFileServerFolder.Text + "/" + tbSetFileName.Text;


            // Собираем и пакуем настройки накатки
            var newSet = new UpdateDbSetting();
            newSet.ServerPathScripts = tbFolderForScripts.Text;
            newSet.TypeConnectionFullName = dbItem.ConType.ToString();
            if (dbItem.AssembyFile != null)
                newSet.AssemplyWithImplementDbConnection = dbItem.AssembyFile.GZipCompress();
            newSet.ConnectionStringModelDb = tbConnectionString.Text;
            newSet.ScriptPartBeforeBodyWithTran = tbPartBeforescript.Text.GetNullIfIsNullOrWhiteSpace();
            newSet.ScriptPartAfterBodyWithTran = tbPartAfterScript.Text.GetNullIfIsNullOrWhiteSpace();
            newSet.ScriptUpdateVer = tbScriptUpdateVer.Text.GetNullIfIsNullOrWhiteSpace();

            var encodedSetting = newSet.SerializeAesEncrypt(newLink.ServerPathToSettings);

            string fileNameSet = tbSetFileName.Text;

            try
            {
                using (var tfsbl = new TFSRoutineBL())
                {
                    var localFileSetPath = Path.Combine(tfsbl.Tempdir, fileNameSet);

                    tfsbl.VersionControl(new Uri(tbTfsProject.Text));
                    tfsbl.MapTempWorkspace(tbSetFileServerFolder.Text);

                    tfsbl.GetLastFile(fileNameSet);

                    var fileExist = File.Exists(localFileSetPath);

                    if (fileExist && !tfsbl.CheckOut(localFileSetPath))
                        throw new Exception("Извлечение настроек неуспешно. Повторите позже");

                    // блокируем от изменений
                    if (fileExist && !tfsbl.LockFile(fileNameSet))
                        throw new Exception("Производится сохранение настроек другим пользователем.");

                    // если есть - удаляем
                    if (fileExist)
                        File.Delete(localFileSetPath);

                    // записываем новый
                    File.WriteAllBytes(localFileSetPath, encodedSetting);

                    if (!fileExist)
                        tfsbl.AddFile(localFileSetPath);

                    List<int> linkedTask = new List<int>();
                    if (taskId.HasValue)
                        linkedTask.Add(taskId.Value);
                    tfsbl.CheckIn("Добавленение настроек версионности", linkedTask);
                }

                this.ItemsInSets.Add(newLink);
            }
            catch (Exception ex)
            {
                String msg = ex.Expand();
                if (ex.GetType().Name == "TargetInvocationException" && ex.InnerException != null)
                    msg = ex.InnerException.Message;
                Dialogs.ErrorF(this, "Сохранение неуспешно" + Environment.NewLine + msg);
                e.Cancel = true;
            }
        }

        private void tbConnectionString_TextChanged(object sender, EventArgs e)
        {
            tbConnectionString_Validating(null, null);
        }
    }
}
