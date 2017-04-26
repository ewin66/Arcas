using System;
using System.Collections;
using System.Text;
using IBM.WMQ;

namespace Arcas.BL.IbmMq
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class IBMMqClient : IDisposable
    {
        private IBMMqClient() { }

        private string host;
        private string managerName;
        private string channelName;
        private string queueName;
        private string userName;
        private string password;

        private MQQueueManager mqManager = null;
        private MQMessage message = null;

        /// <summary>
        /// Создание клиента к MQ 
        /// </summary>
        /// <param name="mqSetting">Настройки очереди</param>
        /// <returns></returns>
        public static IBMMqClient CreateClient(MqSettingGeneric mqSetting)
        {
            return new IBMMqClient()
            {
                host = mqSetting.Host,
                managerName = mqSetting.ManagerName,
                channelName = mqSetting.ChannelName,
                queueName = mqSetting.QueueName,
                userName = mqSetting.UserName,
                password = mqSetting.Password
            };
        }

        private MQQueueManager createManager()
        {
            var properties = new Hashtable();
            properties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED);
            properties.Add(MQC.CONNECTION_NAME_PROPERTY, host);
            properties.Add(MQC.CHANNEL_PROPERTY, channelName);
            if (!string.IsNullOrEmpty(userName))
                properties.Add(MQC.USER_ID_PROPERTY, userName);
            if (!string.IsNullOrEmpty(password))
                properties.Add(MQC.PASSWORD_PROPERTY, password);
            properties.Add(MQC.CONNECT_OPTIONS_PROPERTY, MQC.MQCNO_RECONNECT);

            return new MQQueueManager(managerName, properties);
        }

        /// <summary>
        /// Посыл
        /// </summary>
        public void Send(MqMessageGeneric MqMessage)
        {
            String qname = this.queueName;

            if (String.IsNullOrWhiteSpace(qname))
                throw new ArgumentNullException("Не определено имя очереди");

            try
            {
                message = new MQMessage();
                message.Persistence = MQC.MQPER_PERSISTENT;
                message.Format = MQC.MQFMT_STRING;
                message.CharacterSet = MQC.CODESET_UTF;

                foreach (var prop in MqMessage.AddedProperties)
                    message.SetStringProperty(prop.Key, prop.Value);

                message.Write(Encoding.UTF8.GetBytes(MqMessage.Body));

                SendMqMessage(qname);
                MqMessage.MessageID = message.MessageId;
                message = null;
            }
            catch (MQException mqex)
            {
                message = null;
                throw new InvalidOperationException(String.Format("MQ {0}", mqex.Message));
            }
            catch
            {
                throw;
            }
        }


        private void SendMqMessage(String queueName)
        {
            using (var qm = createManager())
            using (var q = qm.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING))
            {
                q.Put(message);
                message.ClearMessage();
                q.Close();
            }
        }

        public MqMessageGeneric GetNextMessage()
        {
            MqMessageGeneric res = null;

            String qname = this.queueName;

            if (String.IsNullOrWhiteSpace(qname))
                throw new ArgumentNullException("Не определено имя очереди");

            var getMessageOptions = new MQGetMessageOptions();
            getMessageOptions.Options = MQC.MQGMO_WAIT + MQC.MQGMO_SYNCPOINT;
            getMessageOptions.WaitInterval = 100;  // 1 seconds wait​

            try
            {
                if (mqManager == null)
                    mqManager = createManager();

                message = new MQMessage();
                using (var q = mqManager.AccessQueue(qname, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING))
                    q.Get(message, getMessageOptions);

                res = new MqMessageGeneric();
                res.Body = Encoding.UTF8.GetString(message.ReadBytes(message.MessageLength));
                res.MessageID = message.MessageId;
                res.PutDateTime = message.PutDateTime;

                var inames = message.GetPropertyNames("%");
                if (inames != null)
                    while (inames.MoveNext())
                    {
                        String name = inames.Current.ToString();
                        if (name.ToLower().Contains("jms") ||
                            name.ToLower().Contains("mcd"))
                            continue;
                        res.AddedProperties.Add(name, message.GetStringProperty(name));
                    }
            }
            catch (MQException mqex)
            {
                RollbackGet();
                message = null;
                if (mqex.ReasonCode == 2033 && mqex.CompCode == 2) // В очереди нет сообщений
                    return null;
                throw new InvalidOperationException(String.Format("MQ {0}", mqex.Message));
            }
            catch
            {
                RollbackGet();
                throw;
            }

            return res;
        }

        public void RollbackGet()
        {
            if (mqManager != null)
                mqManager.Backout();
        }

        public void CommitGet()
        {
            if (mqManager != null)
                mqManager.Commit();
        }

        #region Члены IDisposable

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (mqManager != null && mqManager.IsConnected)
                mqManager.Disconnect();
            if (mqManager != null)
                ((IDisposable)mqManager).Dispose();
            mqManager = null;
            message = null;
        }

        #endregion
    }
}
