using System;
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

    public class MqUserSetting : MqSettingGeneric
    {

        public MqUserSetting()
        {
            Properties = new BindingList<KeyVal>();
        }

        public MqUserSetting(MqSettingGeneric sets) : this()
        {
            this.Host = sets.Host;
            this.ManagerName = sets.ManagerName;
            this.ChannelName = sets.ChannelName;
            this.QueueName = sets.QueueName;
            this.UserName = sets.UserName;
            this.Password = sets.Password;
        }

        public MqUserSetting(MqSettingGeneric sets, BindingList<KeyVal> addpoplist) : this(sets)
        {
            foreach (var item in addpoplist)
                Properties.Add(item);
        }

        public BindingList<KeyVal> Properties { get; set; }
        public class KeyVal
        {
            public String Key { get; set; }
            public String Value { get; set; }
        }
    }
}