﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using Ppr;

namespace DevTools
{
    /// <summary>
    /// Настройки приложения
    /// </summary>
    public static class DevToolsSettings
    {
        private static SettingsType settings = null;
        public static SettingsType Settings
        {
            get
            {
                if (settings == null)
                    Load();
                return settings;
            }
            private set
            {
                settings = value;
            }
        }

        private static String setFile = Path.Combine(DomainContext.AppDataStorage, "DevTools.config");

        private static void Load()
        {
            // Смена логики получения DomainContext.AppDataStorage. Посему тут заморочка, чтоб предыдущее место прочитал.
            if (!File.Exists(setFile))
                setFile = Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(DomainContext.AppDataStorage), "PPR"), DomainContext.NameEntryAssembly), Path.GetFileName(setFile));
            if (File.Exists(setFile))
                settings = setFile.XMLDeserializeFromFile<SettingsType>();
            if (settings == null)
                settings = new SettingsType();

            setFile = Path.Combine(DomainContext.AppDataStorage, "DevTools.config");

            settings.XMLSerialize(setFile);
        }

        public static void Save()
        {
            settings.XMLSerialize(setFile);
        }

    }

    public class SettingsType
    {
        public SettingsType()
        {
            TfsDbLinks = new List<TfsDbLink>();
        }

        public List<TfsDbLink> TfsDbLinks { get; set; }
    }

    #region Связка TFS-DB

    public class TFSDBList : BindingList<TfsDbLink>
    {
        private TFSDBList(List<TfsDbLink> lfd)
            : base(lfd)
        { }

        public static implicit operator TFSDBList(List<TfsDbLink> lfd)
        {
            return new TFSDBList(lfd);
        }

        public static implicit operator List<TfsDbLink>(TFSDBList tdbl)
        {
            return new List<TfsDbLink>(tdbl);
        }
    }

    public class TfsDbLink
    {
        public TfsDbLink()
        {
            TFS = new TfsSetting();
            DB = new DbSetting();
        }

        public String Name { get; set; }
        public TfsSetting TFS { get; set; }
        public DbSetting DB { get; set; }
    }

    public class TfsSetting
    {
        public String Server { get; set; }
        public String Path { get; set; }

        public override String ToString()
        {
            if (Server.IsNullOrWhiteSpace())
                return "Сервер TFS не указан";
            if (Path.IsNullOrWhiteSpace())
                return String.Format("Не указан путь на сервере {0}", Server);
            return String.Format("{0}/{1}", Server, Path.SubString(2, 100));
        }
    }

    public class DbSetting
    {
        public String Server
        {
            get
            {
                if (scsb == null)
                    return null;
                return scsb.DataSource;
            }
        }
        public String DBName
        {
            get
            {
                if (scsb == null)
                    return null;
                return scsb.InitialCatalog;
            }
        }
        public String ConnectionString
        {
            get
            {
                if (scsb == null)
                    return null;
                return scsb.ToString();
            }
            set
            {
                scsb = new SqlConnectionStringBuilder(value);
            }
        }

        private SqlConnectionStringBuilder scsb = null;


        public override String ToString()
        {
            if (Server.IsNullOrWhiteSpace() || DBName.IsNullOrWhiteSpace())
                return "Соедиенеие не задано";
            return String.Format("[{0}].[{1}]", Server, DBName);
        }
    }

    #endregion

    public class VerDB
    {
        public long VersionBD { get; set; }
        public DateTime DateVersion { get; set; }
        public override string ToString()
        {
            return String.Format("{0} {1:yyyy-MM-dd}", VersionBD.ToString().PadLeft(6, '0'), DateVersion);
        }
    }
}