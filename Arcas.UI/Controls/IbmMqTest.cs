using System;
using System.ComponentModel;
using System.Text;
using Arcas.BL.IbmMq;
using Cav;

namespace Arcas.Controls
{
    public partial class IbmMqTest : TabControlBase
    {
        public IbmMqTest()
        {
            InitializeComponent();
            this.Text = "Взаимодействие с IBM Mq";
            dgvAddProperties.AutoGenerateColumns = false;
            dgvAddProperties.DataSource = addpoplist;
            APKeyCol.DataPropertyName = nameof(MqUserSetting.KeyVal.Key);
            APValueCol.DataPropertyName = nameof(MqUserSetting.KeyVal.Value);

            if (ArcasSetting.Instance.MqSets != null)
            {
                var st = ArcasSetting.Instance.MqSets;

                tbHost.Text = st.Host;
                tbManagerName.Text = st.ManagerName;
                tbChannelName.Text = st.ChannelName;
                tbQueueName.Text = st.QueueName;
                tbUser.Text = st.UserName;
                tbPass.Text = st.Password;

                foreach (var item in st.Properties)
                    addpoplist.Add(item);
            }
        }

        private BindingList<MqUserSetting.KeyVal> addpoplist = new BindingList<MqUserSetting.KeyVal>();

        private Boolean ChekSettings()
        {
            if (tbHost.Text.IsNullOrWhiteSpace())
            {
                Dialogs.ErrorF(this, "Не указан хост");
                return false;
            }

            if (tbQueueName.Text.IsNullOrWhiteSpace())
            {
                Dialogs.ErrorF(this, "Не указано имя очереди");
                return false;
            }

            if (tbChannelName.Text.IsNullOrWhiteSpace())
            {
                Dialogs.ErrorF(this, "Не указано имя канала");
                return false;
            }
            return true;
        }
        private void btSend_Click(object sender, EventArgs e)
        {
            tbMessageID.Text = null;
            tbPutDate.Text = null;

            if (!ChekSettings())
                return;

            MqMessageGeneric msg = new MqMessageGeneric();
            msg.Body = tbBodyMessage.Text.GetNullIfIsNullOrWhiteSpace();
            try
            {
                foreach (var item in addpoplist)
                    msg.AddedProperties.Add(item.Key, item.Value);
            }
            catch
            {
                Dialogs.ErrorF(this, "Указаны повторяющиеся значения ключа");
                return;
            }



            MqSettingGeneric sets = new MqSettingGeneric();
            sets.Host = tbHost.Text.GetNullIfIsNullOrWhiteSpace();
            sets.ManagerName = tbManagerName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.ChannelName = tbChannelName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.QueueName = tbQueueName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.UserName = tbUser.Text.GetNullIfIsNullOrWhiteSpace();
            sets.Password = tbPass.Text.GetNullIfIsNullOrWhiteSpace();

            try
            {
                var clnt = IBMMqClient.CreateClient(sets);
                clnt.Send(msg);

                StringBuilder sb = new StringBuilder(msg.MessageID.Length * 2);
                foreach (var b in msg.MessageID)
                    sb.AppendFormat("{0:X2}", b);

                tbMessageID.Text = sb.ToString();

                ArcasSetting.Instance.MqSets = new MqUserSetting(sets, addpoplist);
                ArcasSetting.Instance.Save();
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
            }

        }

        private void btGetMessage_Click(object sender, EventArgs e)
        {
            tbMessageID.Text = null;
            tbPutDate.Text = null;
            tbBodyMessage.Text = null;

            if (!ChekSettings())
                return;

            MqSettingGeneric sets = new MqSettingGeneric();
            sets.Host = tbHost.Text.GetNullIfIsNullOrWhiteSpace();
            sets.ManagerName = tbManagerName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.ChannelName = tbChannelName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.QueueName = tbQueueName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.UserName = tbUser.Text.GetNullIfIsNullOrWhiteSpace();
            sets.Password = tbPass.Text.GetNullIfIsNullOrWhiteSpace();

            IBMMqClient clnt = null;

            try
            {
                clnt = IBMMqClient.CreateClient(sets);
                var msg = clnt.GetNextMessage();

                if (msg == null)
                {
                    tbBodyMessage.Text = "В очереди нет сообщений";
                }
                else
                {
                    addpoplist.Clear();
                    foreach (var item in msg.AddedProperties)
                        addpoplist.Add(new MqUserSetting.KeyVal() { Key = item.Key, Value = item.Value });

                    StringBuilder sb = new StringBuilder(msg.MessageID.Length * 2);
                    foreach (var b in msg.MessageID)
                        sb.AppendFormat("{0:X2}", b);

                    tbMessageID.Text = sb.ToString();

                    tbPutDate.Text = msg.PutDateTime.ToString("dd.MM.yyyy HH:mm:ss");
                    tbBodyMessage.Text = msg.Body;

                }

                ArcasSetting.Instance.MqSets = new MqUserSetting(sets);
                ArcasSetting.Instance.Save();

                if (chbRollbakGet.Checked)
                    clnt.RollbackGet();
                else
                    clnt.CommitGet();
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
                clnt.RollbackGet();
            }
        }
    }
}
