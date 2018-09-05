using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cav;
using Cav.Tfs;

namespace Arcas.BL.TFS
{
    /// <summary>
    /// Взаимодействие с ТФС
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class TFSRoutineBL : IDisposable
    {
        public TFSRoutineBL()
        {
            // чистим папку
            if (Directory.Exists(Tempdir))
                Utils.DeleteDirectory(Tempdir);

            Directory.CreateDirectory(Tempdir);
        }

        private WrapTfs wrapTfs = new WrapTfs();

        private VersionControlServer vcs = null;


        private const string arcasWorkspaceName = "Arcas Workspace";
        private const string arcasShelveName = "Arcas Roll DB";
        /// <summary>
        /// Получение сервиса управления хранилищем
        /// </summary>
        /// <param name="TfsServer">Url сервера TFS</param>
        /// <returns>Контроллер хранилища</returns>
        public void VersionControl(Uri serverTfs)
        {
            vcs = wrapTfs.VersionControlServerGet(serverTfs);
        }


        public String Tempdir
        {
            get { return tempdir; }
        }

        private String tempdir = Path.Combine(DomainContext.TempPath, Guid.NewGuid().ToString());

        private Workspace tempWorkspace = null;
        private String serverPath = null;

        // Создаем временную рабочую область
        private Workspace getTempWorkspace()
        {
            if (vcs == null)
                throw new ArgumentException("Не установленна связь с TFS");

            if (tempWorkspace != null)
                return tempWorkspace;

            tempWorkspace = wrapTfs.WorkspaceCreate(vcs, Guid.NewGuid().ToString(), arcasWorkspaceName, true);

            return tempWorkspace;
        }

        /// <summary>
        /// Связывание пути на сервере TFS с временной папкой во временной рабочей области
        /// </summary>
        /// <param name="ServerPath"></param>
        public void MapTempWorkspace(String ServerPath)
        {
            try
            {
                if (tempWorkspace != null)
                {
                    var mfdr = wrapTfs.WorkspaceFoldersGet(tempWorkspace).FirstOrDefault();

                    // Если в процесе ремапят, то надо удалить предыдущюю раб.обл.
                    if (mfdr != null &&
                    mfdr.LocalItem != Tempdir &&
                    mfdr.ServerItem != ServerPath)

                    {
                        wrapTfs.WorkspaceUndo(tempWorkspace);
                        wrapTfs.WorkspaceDelete(tempWorkspace);
                        tempWorkspace = null;
                    }

                }
                serverPath = ServerPath;
                // Для этого рабочего пространства папка на сервере сопоставляется с локальной папкой 
                wrapTfs.WorkspaceMap(getTempWorkspace(), serverPath, Tempdir);
            }
            catch
            {
                if (tempWorkspace != null)
                {
                    wrapTfs.WorkspaceUndo(tempWorkspace);
                    wrapTfs.WorkspaceDelete(tempWorkspace);
                    tempWorkspace = null;
                }
                throw;
            }
        }

        public void DownloadFile(string serverPathToSettings, string tempfile)
        {
            wrapTfs.VersionControlServerDownloadFile(vcs, serverPathToSettings, tempfile);
        }

        /// <summary>
        /// Получение последней версии файлав в временной рабочей области
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>Кол-во новых файлов</returns>
        public long GetLastFile(String FileName)
        {
            return wrapTfs.WorkspaceGetLastItem(getTempWorkspace(), serverPath + "/" + FileName);
        }

        /// <summary>
        /// Добавить файл в рабочую область
        /// </summary>
        /// <param name="PathFileName">Полный путь файла, находящийся в папке, замапленой в рабочей области</param>
        public void AddFile(string PathFileName)
        {
            wrapTfs.WorkspaceAddFile(getTempWorkspace(), PathFileName);
        }

        /// <summary>
        /// Возврат изменений в рабочей области
        /// </summary>
        /// <param name="CommentOnCheckIn">Комментарий для изменения</param>
        /// <param name="NumberTasks">Номера задач для чекина</param>
        public void CheckIn(string CommentOnCheckIn, List<int> NumberTasks = null)
        {
            wrapTfs.WorkspaceCheckIn(getTempWorkspace(), CommentOnCheckIn, NumberTasks);
        }

        /// <summary>
        /// Блокировка файла. Псевдо транзанкция
        /// </summary>
        /// <param name="FileName">Имя файла(можно с путем. на сервере)</param>
        /// <param name="Block"></param>
        /// <returns>true - успешно, false - неуспешно</returns>
        public bool LockFile(string FileName)
        {
            return wrapTfs.WorkspaceLockFile(getTempWorkspace(), serverPath + "/" + FileName, LockLevel.CheckOut);
        }

        /// <summary>
        /// Извлечение файла для редактирования
        /// </summary>
        /// <param name="PathFileName">Полный путь файла, находящийся в папке, замапленой в рабочей области</param>
        /// <returns>true - успешно, false - неуспешно</returns>
        public bool CheckOut(string PathFileName)
        {
            return wrapTfs.WorkspaceCheckOut(getTempWorkspace(), PathFileName);
        }

        public bool ExistsShelveset()
        {
            var shlvs = wrapTfs.ShelvesetsCurrenUserLoad(vcs);
            return shlvs.Any(x => x.Name == arcasShelveName);
        }

        public void DeleteShelveset()
        {
            wrapTfs.ShelvesetDelete(vcs, arcasShelveName);
        }

        public void CreateShelveset(string comment, List<int> linkedTask)
        {
            wrapTfs.WorkspaceShelvesetCreate(getTempWorkspace(), arcasShelveName, comment, linkedTask);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            try
            {
                if (tempWorkspace != null)
                {
                    wrapTfs.WorkspaceUndo(tempWorkspace);
                    wrapTfs.WorkspaceDelete(tempWorkspace);
                    tempWorkspace = null;
                }

                if (Directory.Exists(Tempdir))
                    Utils.DeleteDirectory(Tempdir);
            }
            catch
            {
                // очистка мусора неуспешна. жаль. ну а что поделать..
            }
        }
    }
}
