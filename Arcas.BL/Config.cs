using System;
using System.Collections.Generic;
using Arcas.BL.IbmMq;
using Arcas.Settings;
using Cav.Configuration;

namespace Arcas
{
    public class Config : ProgramSettingsBase<Config>
    {
        /// <summary>
        /// Последняя использованная конфигурация при накатке БД
        /// </summary>
        public String SelestedTFSDB { get; set; }

        /// <summary>
        /// Последние успешно использованные настройки при работе с Mq
        /// </summary>
        public MqSettingGeneric MqSets { get; set; }
        /// <summary>
        /// Коллекция настроек связок TFS-DB
        /// </summary>
        public List<TfsDbLink> TfsDbLinks { get; set; }
    }
}
