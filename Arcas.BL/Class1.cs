using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using System.Web.Script.Serialization;

namespace Cav.ProgramSettins2
{
    /// <summary>
    /// Область сохранения настрек
    /// </summary>
    public enum Area
    {
        /// <summary>
        /// Для пользователя
        /// </summary>
        User,
        /// <summary>
        /// Для приложения (В папке сборки)
        /// </summary>
        App,
        /// <summary>
        /// Общее хранилице для всех пользователей
        /// </summary>
        CommonApp
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ProgramSettinsUserAreaAttribute : Attribute
    {
        public ProgramSettinsUserAreaAttribute(Area AreaSetting)
        {
            this.Value = AreaSetting;
        }
        internal Area Value { get; private set; }
    }

    public abstract class ProgramSettingsBase<T> where T : ProgramSettingsBase<T>
    {
        protected ProgramSettingsBase() { }

        private static T _instance = null;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)Activator.CreateInstance(typeof(T));

                    String filename = null;
                    if (!_instance.FileName.IsNullOrWhiteSpace())
                    {
                        _instance.FileName = _instance.FileName.ReplaceInvalidPathChars();
                        filename = _instance.FileName;
                    }

                    if (filename.IsNullOrWhiteSpace())
                        filename = typeof(T).FullName + ".prstg";

                    _instance.fileNameApp = Path.Combine(Path.GetDirectoryName(typeof(T).Assembly.Location), filename);
                    _instance.fileNameUser = Path.Combine(DomainContext.AppDataUserStorage, filename);
                    _instance.fileNameAppCommon = Path.Combine(DomainContext.AppDataCommonStorage, filename);
                    _instance.Reload();
                }

                return _instance;
            }
        }

        private String fileNameApp = null;
        private String fileNameUser = null;
        private String fileNameAppCommon = null;

        protected String FileName;
        private void FromJsonDeserialize(String fileName, PropertyInfo[] prinfs)
        {
            if (!prinfs.Any())
                return;

            if (!File.Exists(fileName))
                return;

            List<PropNameVal> pvl = null;

            var jss = new JavaScriptSerializer();
            pvl = jss.Deserialize<List<PropNameVal>>(File.ReadAllText(fileName));

            if (pvl == null)
            {
                File.Delete(fileName);
                return;
            }

            foreach (var pv in pvl)
            {
                var pi = prinfs.FirstOrDefault(x => x.Name == pv.Name);
                if (pi == null)
                    continue;
                pi.SetValue(this, jss.Deserialize(pv.Value, pi.PropertyType));
            }
        }

        private void ToJsonSerialize(String fileName, List<PropNameVal> props)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            if (!props.Any())
                return;
            var jss = new JavaScriptSerializer();
            var jsonStr = jss.Serialize(props);

            File.WriteAllText(fileName, jsonStr);
        }


        private class PropNameVal
        {
            public String Name { get; set; }
            public String Value { get; set; }
        }

        public void Reload()
        {
            lock (this)
            {
                PropertyInfo[] prinfs = this.GetType().GetProperties();

                foreach (var pinfo in prinfs)
                    pinfo.SetValue(this, pinfo.PropertyType.GetDefault());

                FromJsonDeserialize(fileNameApp,
                    prinfs
                    .Where(pinfo =>
                        pinfo.GetCustomAttribute<ProgramSettinsUserAreaAttribute>() != null && pinfo.GetCustomAttribute<ProgramSettinsUserAreaAttribute>().Value == Area.App
                        ).ToArray()
                    );

                FromJsonDeserialize(fileNameAppCommon,
                    prinfs
                    .Where(pinfo =>
                        pinfo.GetCustomAttribute<ProgramSettinsUserAreaAttribute>() != null && pinfo.GetCustomAttribute<ProgramSettinsUserAreaAttribute>().Value == Area.CommonApp
                        ).ToArray()
                    );

                FromJsonDeserialize(fileNameUser,
                    prinfs
                    .Where(pinfo =>
                        pinfo.GetCustomAttribute<ProgramSettinsUserAreaAttribute>() == null || pinfo.GetCustomAttribute<ProgramSettinsUserAreaAttribute>().Value == Area.User
                        ).ToArray()
                    );
            }
        }

        public void Save()
        {
            lock (this)
            {
                PropertyInfo[] prinfs = this.GetType().GetProperties();

                List<PropNameVal> appVal = new List<PropNameVal>();
                List<PropNameVal> appCommonVal = new List<PropNameVal>();
                List<PropNameVal> userVal = new List<PropNameVal>();

                var jss = new JavaScriptSerializer();

                foreach (var pinfo in prinfs)
                {
                    Object val = pinfo.GetValue(this);
                    if (val == null)
                        continue;

                    var psatr = pinfo.GetCustomAttribute<ProgramSettinsUserAreaAttribute>() ?? new ProgramSettinsUserAreaAttribute(Area.User);

                    switch (psatr.Value)
                    {
                        case Area.User:
                            userVal.Add(new PropNameVal() { Name = pinfo.Name, Value = jss.Serialize(val) });
                            break;
                        case Area.App:
                            appVal.Add(new PropNameVal() { Name = pinfo.Name, Value = jss.Serialize(val) });
                            break;
                        case Area.CommonApp:
                            appCommonVal.Add(new PropNameVal() { Name = pinfo.Name, Value = jss.Serialize(val) });
                            break;
                        default:
                            throw new ArgumentException("ProgramSettinsUserAreaAttribute.Value");
                    }
                }

                ToJsonSerialize(fileNameUser, userVal);
                ToJsonSerialize(fileNameAppCommon, appCommonVal);
                ToJsonSerialize(fileNameApp, appVal);
            }
        }
    }
}