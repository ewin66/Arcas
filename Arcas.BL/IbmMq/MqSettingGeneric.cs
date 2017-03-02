using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Arcas.BL.IbmMq
{
    public class MqSettingGeneric
    {
        public string Host { get; set; }
        public string ManagerName { get; set; }
        public string ChannelName { get; set; }
        public string QueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}