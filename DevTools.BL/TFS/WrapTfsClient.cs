﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Cav.Ttf
{
    internal static class ReflectHelpers
    {
        public static Object GetPropertyValue(this object obj, String PropertyName)
        {
            return obj.GetType().GetProperty(PropertyName).GetValue(obj);
        }

        public static Object GetStaticPropertyValue(this Assembly asm, String className, String PropertyName)
        {
            return asm.ExportedTypes.Single(x => x.Name == className).GetProperty(PropertyName).GetValue(null);
        }

        public static void SetPropertyValue(this object obj, String PropertyName, Object value)
        {
            obj.GetType().GetProperty(PropertyName).SetValue(obj, value);
        }

        public static Object CreateInstance(this Assembly asm, String className, params object[] args)
        {
            Type clType = asm.ExportedTypes.Single(x => x.Name == className);
            return Activator.CreateInstance(clType, args);
        }

        public static Object GetEnumValue(this Assembly asm, String enumTypeName, String VelueName)
        {
            Type rtType = asm.ExportedTypes.Single(x => x.Name == enumTypeName);
            return Enum.Parse(rtType, VelueName);
        }

        public static Object InvokeMethod(this Object obj, String methidName, params object[] arg)
        {
            if (arg == null)
                arg = new object[0];
            var minfo = obj.GetType().GetMethod(methidName, arg.Select(x => x.GetType()).ToArray());
            return minfo.Invoke(obj, arg);
        }
    }

    public enum LockLevel
    {
        None,
        Checkin,
        CheckOut,
        Unchanged
    }

    #region объекты обмена

    public struct WorkspaceInfo
    {
        internal Object WSI { get; set; }
    }
    public struct VersionControlServer
    {
        internal Object VCS { get; set; }
    }

    public struct Workspace
    {
        internal Object WS { get; set; }
    }

    public struct VersionSpec
    {
        internal Object VS { get; set; }
    }

    public struct QueryHistoryParameters
    {
        internal Object QHP { get; set; }
    }

    public struct Changeset
    {
        public int ChangesetId { get; internal set; }
        public DateTime CreationDate { get; internal set; }
    }

    public struct PendingChange
    {
        internal Object PC { get; set; }
    }

    public class WorkingFolder
    {
        public String ServerItem { get; internal set; }
        public String LocalItem { get; internal set; }
        public bool IsCloaked { get; internal set; }
    }

    #endregion

    public class WrapTfs
    {
        private const string tfsClient12 = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\ReferenceAssemblies\v2.0\";
        private const string tfsClient14 = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\";
        private static String pathTfsdll = null;

        private const string tfsClientDll = "Microsoft.TeamFoundation.Client.dll";
        private const string tfsVersionClientDll = "Microsoft.TeamFoundation.VersionControl.Client.dll";
        private const string tfsVersionControlCommonDLL = "Microsoft.TeamFoundation.VersionControl.Controls.Common.dll";

        private static Assembly tfsClientAssembly = null;
        private static Assembly tfsVersionClientAssembly = null;
        private static Assembly tfsVersionControlCommonAssembly = null;

        static WrapTfs()
        {
            if (Directory.Exists(tfsClient14))
                pathTfsdll = tfsClient14;
            if (pathTfsdll == null && Directory.Exists(tfsClient12))
                pathTfsdll = tfsClient12;
            if (pathTfsdll == null)
                throw new FileNotFoundException(String.Format("Not found TFS assemblys on path {0}, {1}.", tfsClient14, tfsClient12));

            AppDomain.CurrentDomain.AssemblyResolve += WrapTfs.CurrentDomain_AssemblyResolve;

            tfsClientAssembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(Path.Combine(pathTfsdll, tfsClientDll)));
            tfsVersionClientAssembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(Path.Combine(pathTfsdll, tfsVersionClientDll)));
            tfsVersionControlCommonAssembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(Path.Combine(pathTfsdll, tfsVersionControlCommonDLL)));
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == args.Name);
            if (asly != null)
                return asly;

            string assemblyFile = (args.Name.Contains(','))
                ? args.Name.Substring(0, args.Name.IndexOf(','))
                : args.Name;

            assemblyFile += ".dll";
            var path = Path.Combine(pathTfsdll, assemblyFile);
            if (!File.Exists(path))
            {
                var culture = args.Name.Split(new Char[] { ',' }).Where(x => x.Contains("Culture")).FirstOrDefault();
                if (culture == null)
                    throw new FileLoadException($"Not define culture for {args.Name}");
                var targetculture = culture.Split(new char[] { '=', '-' }).Where(x => !x.Contains("Culture")).FirstOrDefault();
                if (targetculture == null)
                    throw new FileLoadException($"Not compute culture for {args.Name}");
                path = Path.Combine(Path.Combine(pathTfsdll, targetculture), assemblyFile);
            }
            return Assembly.LoadFile(path);

        }

        private Object ws = null;
        private Object WorkstationGet()
        {
            if (ws != null)
                return ws;
            ws = tfsVersionClientAssembly.GetStaticPropertyValue("Workstation", "Current");
            return ws;
        }

        public void WorkstationReloadCache()
        {
            WorkstationGet().InvokeMethod("ReloadCache");
        }

        private Object TeamProjectCollectionGet(Uri serverUri)
        {
            Type TTPCFType = tfsClientAssembly.ExportedTypes.Single(x => x.Name == "TfsTeamProjectCollectionFactory");
            var minfo = TTPCFType.GetMethod("GetTeamProjectCollection", new Type[] { typeof(Uri) });
            return minfo.Invoke(null, new object[] { serverUri });
        }

        public WorkspaceInfo WorkspaceInfoGet(String localFileName)
        {
            return new WorkspaceInfo() { WSI = WorkstationGet().InvokeMethod("GetLocalWorkspaceInfo", localFileName) };
        }

        public VersionControlServer VersionControlServerGet(WorkspaceInfo workspaceInfo)
        {
            Uri servUri = (Uri)workspaceInfo.WSI.GetPropertyValue("ServerUri");
            return VersionControlServerGet(servUri);
        }

        public VersionControlServer VersionControlServerGet(Uri serverUri)
        {
            var tpc = TeamProjectCollectionGet(serverUri);
            Type vcsType = tfsVersionClientAssembly.ExportedTypes.Single(x => x.Name == "VersionControlServer");

            var res = new VersionControlServer();
            res.VCS = tpc.InvokeMethod("GetService", vcsType);

            return res;
        }

        public Workspace WorkspaceGet(VersionControlServer vcs, WorkspaceInfo wsi)
        {
            var res = new Workspace();
            res.WS = vcs.VCS.InvokeMethod("GetWorkspace", wsi.WSI);
            return res;
        }

        public VersionSpec WorkspaceVersionSpecCreate(Workspace ws)
        {
            var res = new VersionSpec();
            res.VS = tfsVersionClientAssembly.CreateInstance("WorkspaceVersionSpec", ws.WS);
            return res;
        }

        public QueryHistoryParameters QueryHistoryParametersCreate(String localFileName, VersionSpec vs)
        {
            var RecursionTypeFull = tfsVersionClientAssembly.GetEnumValue("RecursionType", "Full");
            var res = new QueryHistoryParameters();
            res.QHP = tfsVersionClientAssembly.CreateInstance("QueryHistoryParameters", localFileName, RecursionTypeFull);
            res.QHP.SetPropertyValue("ItemVersion", vs.VS);
            res.QHP.SetPropertyValue("VersionEnd", vs.VS);
            res.QHP.SetPropertyValue("MaxResults", 1);
            return res;
        }

        public Changeset ChangesetGet(VersionControlServer vsc, QueryHistoryParameters qhp)
        {
            var cscolobj = ((IEnumerable)vsc.VCS.InvokeMethod("QueryHistory", qhp.QHP)).GetEnumerator();
            object csobj = null;
            if (cscolobj.Current == null)
                cscolobj.MoveNext();
            csobj = cscolobj.Current;

            var res = new Changeset();
            res.ChangesetId = (int)csobj.GetPropertyValue("ChangesetId");
            res.CreationDate = (DateTime)csobj.GetPropertyValue("CreationDate");
            return res;
        }

        public Workspace WorkspaceCreate(
            VersionControlServer vcs,
            String workspaceName,
            String workspaceComment,
            Boolean workspaceLocationOnServer)
        {
            var tpc = vcs.VCS.GetPropertyValue("TeamProjectCollection");
            var cwp = tfsVersionClientAssembly.CreateInstance("CreateWorkspaceParameters", workspaceName);
            cwp.SetPropertyValue("Comment", workspaceComment);
            cwp.SetPropertyValue("OwnerName", tpc.GetPropertyValue("AuthorizedIdentity").GetPropertyValue("UniqueName"));

            object localion = null;
            if (workspaceLocationOnServer)
                localion = tfsVersionControlCommonAssembly.GetEnumValue("WorkspaceLocation", "Server");
            else
                localion = tfsVersionControlCommonAssembly.GetEnumValue("WorkspaceLocation", "Local");

            cwp.SetPropertyValue("Location", localion);

            var res = new Workspace();
            res.WS = vcs.VCS.InvokeMethod("CreateWorkspace", cwp);
            return res;
        }

        public void WorkspaceMap(
            Workspace ws,
            string serverPath,
            string localPath
            )
        {
            ws.WS.InvokeMethod("Map", serverPath, localPath);
        }

        public void WorkspaceDelete(Workspace ws)
        {
            ws.WS.InvokeMethod("Delete");
        }

        /// <summary>
        /// Добавить файл в рабочую область
        /// </summary>
        /// <param name="localPathFile">Полный путь файла, находящийся в папке, замапленой в рабочей области</param>        
        public void WorkspaceAddFile(Workspace ws, string localPathFile)
        {
            ws.WS.InvokeMethod("PendAdd", localPathFile);
        }

        /// <summary>
        /// Извлечение файла для редактирования
        /// </summary>
        /// <param name="localPathFile">Полный путь файла, находящийся в папке, замапленой в рабочей области</param>
        /// <returns>true - успешно, false - неуспешно</returns>
        public bool WorkspaceCheckOut(Workspace ws, string localPathFile)
        {
            return (int)ws.WS.InvokeMethod("PendEdit", localPathFile) != 0;
        }

        /// <summary>
        /// Возврат изменений в рабочей области
        /// </summary>
        /// <param name="commentOnCheckIn">Комментарий для изменения</param>
        /// <param name="numberTasks">Номера задач для чекина</param>
        public void WorkspaceCheckIn(Workspace ws, string commentOnCheckIn, List<int> numberTasks = null)
        {
            var gpc = WorkspaceGetPendingChanges(ws);
            var wscp = tfsVersionClientAssembly.CreateInstance("WorkspaceCheckInParameters", gpc.PC, commentOnCheckIn);
            if (numberTasks != null)
            {
                // TODO Получить рабочие элементы
                //wscp.AssociatedWorkItems
            }

            ws.WS.InvokeMethod("CheckIn", wscp);
        }

        private PendingChange WorkspaceGetPendingChanges(Workspace ws)
        {
            var res = new PendingChange();
            res.PC = ws.WS.InvokeMethod("GetPendingChanges");
            return res;
        }

        public void WorkspaceUndo(Workspace ws)
        {
            var pc = WorkspaceGetPendingChanges(ws);
            int cnt = (int)pc.PC.GetPropertyValue("Length");
            if (cnt == 0)
                return;
            ws.WS.InvokeMethod("Undo", pc.PC);
        }

        public List<WorkingFolder> WorkspaceFoldersGet(Workspace ws)
        {
            var res = new List<WorkingFolder>();
            var flds = (ws.WS.GetPropertyValue("Folders") as IEnumerable).GetEnumerator();
            while (flds.MoveNext())
            {
                var crnt = flds.Current;
                if (crnt == null)
                    continue;
                var fi = new WorkingFolder();
                fi.ServerItem = (String)crnt.GetPropertyValue("ServerItem");
                fi.LocalItem = (String)crnt.GetPropertyValue("LocalItem");
                fi.IsCloaked = (Boolean)crnt.GetPropertyValue("IsCloaked");
                res.Add(fi);
            }

            return res;
        }

        public bool WorkspaceLockFile(Workspace ws, string serverPathItem, LockLevel lockLevel)
        {
            bool res = false;
            try
            {
                var llenum = tfsVersionClientAssembly.GetEnumValue("LockLevel", lockLevel.ToString());
                res = (int)ws.WS.InvokeMethod("SetLock", serverPathItem, llenum) != 0;
            }
            catch
            {

            }
            return res;
        }

        public long WorkspaceGetLastItem(Workspace ws, String serverItemPath)
        {
            var RecursionTypeFull = tfsVersionClientAssembly.GetEnumValue("RecursionType", "Full");
            var ItemSpec = tfsVersionClientAssembly.CreateInstance("ItemSpec", serverItemPath, RecursionTypeFull);

            var VersionSpecLatest = tfsVersionClientAssembly.GetStaticPropertyValue("VersionSpec", "Latest");

            var GetRequest = tfsVersionClientAssembly.CreateInstance("GetRequest", ItemSpec, VersionSpecLatest);

            var GetOptionsType = tfsVersionClientAssembly.ExportedTypes.Single(x => x.Name == "GetOptions");
            int GetOptionsGetAll = (int)tfsVersionClientAssembly.GetEnumValue("GetOptions", "GetAll");
            int GetOptionsOverwrit = (int)tfsVersionClientAssembly.GetEnumValue("GetOptions", "Overwrite");
            var GetOptionsValue = Convert.ChangeType(GetOptionsGetAll + GetOptionsOverwrit, GetOptionsType);

            var GetLastFile = ws.WS.InvokeMethod("Get", GetRequest, GetOptionsValue);

            return (long)GetLastFile.GetPropertyValue("NumFiles");
        }

        public Uri ShowTeamProjectPicker(IWin32Window parentWindow)
        {
            var TeamProjectPickerModeNoProject = tfsClientAssembly.GetEnumValue("TeamProjectPickerMode", "NoProject");

            var tpp = tfsClientAssembly.CreateInstance("TeamProjectPicker", TeamProjectPickerModeNoProject, false);
            var dr = (DialogResult)tpp.InvokeMethod("ShowDialog", parentWindow);
            if (dr != DialogResult.OK)
                return null;

            return (Uri)tpp.GetPropertyValue("SelectedTeamProjectCollection").GetPropertyValue("Uri");
        }

        public String ShowDialogChooseServerFolder(IWin32Window parentWindow, VersionControlServer vcs, String initalPath)
        {
            var dcsf = tfsVersionControlCommonAssembly.CreateInstance("DialogChooseServerFolder", vcs.VCS, initalPath);

            dcsf.SetPropertyValue("ShowInTaskbar", false);
            var dr = (DialogResult)dcsf.InvokeMethod("ShowDialog", parentWindow);
            if (dr != DialogResult.OK)
                return null;

            return (String)dcsf.GetPropertyValue("CurrentServerItem");
        }

        //http://stackoverflow.com/questions/25923734/how-can-i-retrieve-a-list-of-workitems-from-tfs-in-c


        //public Object get_querys()
        //{
        //        string selectedProject = this.listProjects.SelectedItem.ToString();
        //        TfsTeamProjectCollection teamProjectCollection = TFSDetail.GetTeamProjectDetails(UrlPath);
        //if (teamProjectCollection != null)
        //{
        //     Project detailsOfTheSelectedProject = null;
        //        WorkItemStore workItemStore =
        //              (WorkItemStore)teamProjectCollection.GetService(typeof(WorkItemStore));

        //        string folder = "My Queries";
        //    var project = workItemStore.Projects[selectedProject];
        //    QueryHierarchy queryHierarchy = project.QueryHierarchy;
        //    var queryFolder = queryHierarchy as QueryFolder;
        //    QueryItem queryItem = queryFolder[folder];
        //    queryFolder = queryItem as QueryFolder;
        //    foreach (var item in queryFolder)
        //    {
        //        listQueries.Items.Add(item.Name);
        //    }
        //}


        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        ///Handles nested query folders    
        //private static Guid FindQuery(QueryFolder folder, string queryName)
        //{
        //    foreach (var item in folder)
        //    {
        //        if (item.Name.Equals(queryName, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            return item.Id;
        //        }

        //        var itemFolder = item as QueryFolder;
        //        if (itemFolder != null)
        //        {
        //            var result = FindQuery(itemFolder, queryName);
        //            if (!result.Equals(Guid.Empty))
        //            {
        //                return result;
        //            }
        //        }
        //    }
        //    return Guid.Empty;
        //}

        //static void Main(string[] args)
        //{
        //    var collectionUri = new Uri("http://TFS/tfs/DefaultCollection");
        //    var server = new TfsTeamProjectCollection(collectionUri);
        //    var workItemStore = server.GetService<WorkItemStore>();

        //    var teamProject = workItemStore.Projects["TeamProjectName"];

        //    var x = teamProject.QueryHierarchy;
        //    var queryId = FindQuery(x, "QueryNameHere");

        //    var queryDefinition = workItemStore.GetQueryDefinition(queryId);
        //    var variables = new Dictionary<string, string>() { { "project", "TeamProjectName" } };

        //    var result = workItemStore.Query(queryDefinition.QueryText, variables); //https://msdn.microsoft.com/ru-ru/library/bb140400(v=vs.120).aspx
        //}
    }
}


