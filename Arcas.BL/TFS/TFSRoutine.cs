using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cav;
using Cav.BaseClases;
using Cav.Tfs;

namespace Arcas.BL.TFS
{
    /// <summary>
    /// Взаимодействие с ТФС
    /// </summary>
    public class TFSRoutineBL : BusinessLogicBase
    {
        public TFSRoutineBL()
        {
            // чистим папку
            if (Directory.Exists(Tempdir))
                Utils.DeleteDirectory(Tempdir);

            Directory.CreateDirectory(Tempdir);
        }

        private WrapTfs wrapTfs = new WrapTfs();

        private VersionControlServer? vcs = null;
        /// <summary>
        /// Получение сервиса управления хранилищем
        /// </summary>
        /// <param name="TfsServer">Url сервера TFS</param>
        /// <returns>Контроллер хранилища</returns>
        public void VersionControl(String TfsServer)
        {
            vcs = wrapTfs.VersionControlServerGet(new Uri(TfsServer));
        }


        public String Tempdir
        {
            get { return tempdir; }
        }

        private String tempdir = Path.Combine(DomainContext.TempPath, Guid.NewGuid().ToString());

        private Workspace? tempWorkspace = null;
        private String serverPath = null;

        // Создаем временную рабочую область
        private Workspace getTempWorkspace()
        {
            if (vcs == null)
                throw new ArgumentException("Не установленна связь с TFS");

            if (tempWorkspace != null)
                return tempWorkspace.Value;

            tempWorkspace = wrapTfs.WorkspaceCreate(vcs.Value, Guid.NewGuid().ToString(), "Arcas Workspace", true);

            return tempWorkspace.Value;
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
                    var mfdr = wrapTfs.WorkspaceFoldersGet(tempWorkspace.Value).FirstOrDefault();

                    // Если в процесе ремапят, то надо удалить предыдущюю раб.обл.
                    if (mfdr != null &&
                    mfdr.LocalItem != Tempdir &&
                    mfdr.ServerItem != ServerPath)

                    {
                        wrapTfs.WorkspaceUndo(tempWorkspace.Value);
                        wrapTfs.WorkspaceDelete(tempWorkspace.Value);
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
                    wrapTfs.WorkspaceUndo(tempWorkspace.Value);
                    wrapTfs.WorkspaceDelete(tempWorkspace.Value);
                    tempWorkspace = null;
                }
                throw;
            }
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

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (tempWorkspace != null)
                {
                    wrapTfs.WorkspaceUndo(tempWorkspace.Value);
                    wrapTfs.WorkspaceDelete(tempWorkspace.Value);
                    tempWorkspace = null;
                }

                if (Directory.Exists(Tempdir))
                    Utils.DeleteDirectory(Tempdir);
            }
            catch
            {
                // очистка мусора неуспешна. жаль. ну а что поделать..
            }

            base.Dispose(disposing);
        }
    }
}
