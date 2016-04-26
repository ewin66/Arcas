using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Ppr;
using Ppr.BaseClases;

namespace DevTools.BL.TFS
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

        /// <summary>
        /// Получение сервиса управления хранилищем
        /// </summary>
        /// <param name="TfsServer">Url сервера TFS</param>
        /// <returns>Контроллер хранилища</returns>
        public VersionControlServer VersionControl(String TfsServer)
        {
            if (tpc == null)
                ProjectCollection(TfsServer);
            return tpc.GetService<VersionControlServer>();
        }

        //private VersionControlServer verControl
        //{
        //    get
        //    {
        //        return tpc.GetService<VersionControlServer>();
        //    }
        //}

        /// <summary>
        /// Получение коллекции проектов на сервере
        /// </summary>
        /// <param name="TfsServer">Url сервера TFS</param>
        /// <returns>Коллекция проектов</returns>
        private TfsTeamProjectCollection ProjectCollection(String TfsServer)
        {
            if (tpc == null)
                tpc = new TfsTeamProjectCollection(new Uri(TfsServer));
            return tpc;
        }

        public String Tempdir
        {
            get { return tempdir; }
        }

        private String tempdir = Path.Combine(DomainContext.TempPath, Guid.NewGuid().ToString());

        private TfsTeamProjectCollection tpc = null;
        private Workspace tempWorkspace = null;
        private String serverPath = null;

        // Создаем временную рабочую область
        private Workspace getTempWorkspace()
        {
            if (tpc == null)
                throw new Exception("");

            if (tempWorkspace == null)
            {
                var cwp = new CreateWorkspaceParameters(Guid.NewGuid().ToString());
                cwp.Comment = "DevTools Workspace";
                cwp.OwnerName = tpc.AuthorizedIdentity.UniqueName;
                cwp.Location = WorkspaceLocation.Server;
                tempWorkspace = tpc.GetService<VersionControlServer>().CreateWorkspace(cwp);
            }

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
                // Если в процесе ремапят, то надо удалить предыдущюю раб.обл.
                if (tempWorkspace != null &&
                    tempWorkspace.Folders[0].LocalItem != Tempdir &&
                    tempWorkspace.Folders[0].ServerItem != ServerPath)
                {
                    if (tempWorkspace.GetPendingChanges().Length > 0)
                        tempWorkspace.Undo(tempWorkspace.GetPendingChanges());
                    tempWorkspace.Delete();
                    tempWorkspace = null;
                }

                serverPath = ServerPath;
                // Для этого рабочего пространства папка на сервере сопоставляется с локальной папкой 
                getTempWorkspace().Map(serverPath, Tempdir);
            }
            catch
            {
                if (tempWorkspace != null)
                {
                    if (tempWorkspace.GetPendingChanges().Length > 0)
                        tempWorkspace.Undo(tempWorkspace.GetPendingChanges());
                    tempWorkspace.Delete();
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
            // Создание ItemSpec для определения списка получаемых файлов и папок 
            // Получение всего содержимого папки на сервере 
            var fileRequest = new GetRequest(new ItemSpec(serverPath + "/" + FileName, RecursionType.None), VersionSpec.Latest);
            // Получение новейшего кода 
            return getTempWorkspace().Get(fileRequest, GetOptions.GetAll | GetOptions.Overwrite).NumFiles;
        }

        /// <summary>
        /// Добавить файл в рабочую область
        /// </summary>
        /// <param name="PathFileName">Полный путь файла, находящийся в папке, замапленой в рабочей области</param>
        public void AddFile(string PathFileName)
        {
            getTempWorkspace().PendAdd(PathFileName);
        }

        /// <summary>
        /// Возврат изменений в рабочей области
        /// </summary>
        /// <param name="CommentOnCheckIn">Комментарий для изменения</param>
        /// <param name="NumberTasks">Номера задач для чекина</param>
        public void CheckIn(string CommentOnCheckIn, List<int> NumberTasks = null)
        {
            var wscp = new WorkspaceCheckInParameters(getTempWorkspace().GetPendingChanges(), CommentOnCheckIn);
            if (NumberTasks != null)
            {
                // TODO Получить рабочие элементы
                //wscp.AssociatedWorkItems
            }

            getTempWorkspace().CheckIn(wscp);
        }

        /// <summary>
        /// Блокировка файла. Псевдо транзанкция
        /// </summary>
        /// <param name="FileName">Имя файла(можно с путем. на сервере)</param>
        /// <param name="Block"></param>
        /// <returns>true - успешно, false - неуспешно</returns>
        public bool LockFile(string FileName)
        {
            bool res = false;
            try
            {
                var rsl = getTempWorkspace().SetLock(serverPath + "/" + FileName, LockLevel.CheckOut);
                res = rsl != 0;
            }
            catch
            {

            }
            return res;
        }

        /// <summary>
        /// Извлечение файла для редактирования
        /// </summary>
        /// <param name="PathFileName">Полный путь файла, находящийся в папке, замапленой в рабочей области</param>
        /// <returns>true - успешно, false - неуспешно</returns>
        public bool CheckOut(string PathFileName)
        {
            var xx = getTempWorkspace().PendEdit(PathFileName);
            return xx != 0;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (tempWorkspace != null)
                {
                    if (tempWorkspace.GetPendingChanges().Length > 0)
                        tempWorkspace.Undo(tempWorkspace.GetPendingChanges());
                    tempWorkspace.Delete();
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
