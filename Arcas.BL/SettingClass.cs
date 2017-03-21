using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Arcas.Settings
{
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

    #endregion
    public class VerDB
    {
        public long VersionBD { get; set; }
        public DateTime DateVersion { get; set; }
        public override string ToString()
        {
            return String.Format("{0} {1:yyyy.MM.dd}", VersionBD.ToString().PadLeft(2, '0'), DateVersion);
        }

        public static implicit operator String(VerDB ver)
        {
            if (ver == null)
                return null;

            return ver.ToString();
        }
    }


    public class TfsDbLink
    {
        public String Name { get; set; }
        public Uri ServerUri { get; set; }
        public String ServerPathToSettings { get; set; }
    }

    public class UpdateDbSetting
    {
        /// <summary>
        /// Путь на сервере СКВ, куда складываются скрипты
        /// </summary>
        public String ServerPathScripts { get; set; }
        /// <summary>
        /// Полное имя типа (с пространством имен) коннекшена
        /// </summary>        
        public String TypeConnectionFullName { get; set; }
        /// <summary>
        /// Бинарный образ сборки, содержащей тип конекшена к БД, который реализует DbConnection. Null для SqlConnection
        /// </summary>
        public byte[] AssemplyWithImplementDbConnection { get; set; }
        /// <summary>
        /// Строка соединения с БД для накатки (модельная БД)
        /// </summary>
        public String ConnectionStringModelDb { get; set; }
        /// <summary>
        /// Часть скрипта, идущая перед телом версии при наличии транзакции
        /// </summary>
        public String ScriptPartBeforeBodyWithTran { get; set; }
        /// <summary>
        /// Часть скрипта, идущая после тела версии при наличии транзакции
        /// </summary>
        public String ScriptPartAfterBodyWithTran { get; set; }
        /// <summary>
        /// Скрипт измемения значения версии БД
        /// </summary>
        public string ScriptUpdateVer { get; set; }

        //        Текстовки оборачиваются в String.Format(...
        //Параметры:
        //0 - сгенернная версия (int)
        //1 - дата генерации(DateTime)
        //2 - коментарий пользователя к скрипту(replace '/r/n' на '/r/n--'  то есть. строки начинаются с  "--")
    }
}
