using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Arcas.BL.IbmMq;
using Cav;

namespace Arcas.Controls
{
    public partial class IbmMqTest : TabControlBase
    {
        public IbmMqTest(
            MqBL mqBL)
        {
            InitializeComponent();
            this.Text = "Взаимодействие с IBM Mq";
            dgvAddProperties.AutoGenerateColumns = false;
            dgvAddProperties.DataSource = addpoplist;
            APKeyCol.DataPropertyName = nameof(KeyValuePair<string, string>.Key);
            APValueCol.DataPropertyName = nameof(KeyValuePair<string, string>.Value);

            if ((ArcasSetting.Instance.MqSets ?? Config.Instance.MqSets) != null)
            {
                var st = ArcasSetting.Instance.MqSets ?? Config.Instance.MqSets;

                tbHost.Text = st.Host;
                tbManagerName.Text = st.ManagerName;
                tbChannelName.Text = st.ChannelName;
                tbQueueName.Text = st.QueueName;
                tbUser.Text = st.UserName;
                tbPass.Text = st.Password;
            }

            this.mqBL = mqBL;
        }

        private BindingList<KeyValuePair<String, String>> addpoplist = new BindingList<KeyValuePair<string, string>>();
        private MqBL mqBL;

        private void btSend_Click(object sender, EventArgs e)
        {
            tbMessageID.Text = null;
            tbPutDate.Text = null;

            try
            {
                MqSettingGeneric sets = CreateMqSetting();

                tbMessageID.Text = mqBL.Send(
                    sets,
                    tbBodyMessage.Text.GetNullIfIsNullOrWhiteSpace(),
                    addpoplist.ToDictionary(x => x.Key, x => x.Value)
                    );


                Config.Instance.MqSets =
             ArcasSetting.Instance.MqSets = sets;

                ArcasSetting.Instance.Save();
                Config.Instance.Save();
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
            }
        }

        private MqSettingGeneric CreateMqSetting()
        {
            MqSettingGeneric sets = new MqSettingGeneric();
            sets.Host = tbHost.Text.GetNullIfIsNullOrWhiteSpace();
            sets.ManagerName = tbManagerName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.ChannelName = tbChannelName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.QueueName = tbQueueName.Text.GetNullIfIsNullOrWhiteSpace();
            sets.UserName = tbUser.Text.GetNullIfIsNullOrWhiteSpace();
            sets.Password = tbPass.Text.GetNullIfIsNullOrWhiteSpace();
            return sets;
        }

        private void btGetMessage_Click(object sender, EventArgs e)
        {
            tbMessageID.Text = null;
            tbPutDate.Text = null;
            tbBodyMessage.Text = null;

            MqSettingGeneric sets = CreateMqSetting();

            try
            {
                var msg = mqBL.Get(sets, chbRollbakGet.Checked);

                addpoplist.Clear();
                foreach (var item in msg.AddedProperties)
                    addpoplist.Add(item);

                StringBuilder sb = new StringBuilder(msg.MessageID.Length * 2);
                foreach (var b in msg.MessageID)
                    sb.AppendFormat("{0:X2}", b);

                tbMessageID.Text = sb.ToString();

                tbPutDate.Text = msg.PutDateTime.ToString("dd.MM.yyyy HH:mm:ss");
                tbBodyMessage.Text = msg.Body;

                Config.Instance.MqSets =
                    ArcasSetting.Instance.MqSets = sets;

                ArcasSetting.Instance.Save();
                Config.Instance.Save();
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
            }
        }

        private string FillAddProp(String xml)
        {
            addpoplist.Clear();
            var xl = XDocument.Parse(xml);

            if (xl.Root.Name.LocalName == "Message")
            {
                addpoplist.Add(new KeyValuePair<string, string>("Method", "SendRequest"));

                XName enoNode = xl.Root.Name.Namespace + "ServiceTypeCode";

                var sn = xl.Descendants(enoNode).FirstOrDefault();
                if (sn == null)
                    throw new Exception("Не найден элемент ServiceTypeCode");

                addpoplist.Add(new KeyValuePair<string, string>("ServiceTypeCode", sn.Value));
            }

            if (xl.Root.Name.LocalName == "StatusMessage")
            {
                addpoplist.Add(new KeyValuePair<string, string>("Method", "SetFilesAndStatus"));

                XName enoNode = xl.Root.Name.Namespace + "ServiceTypeCode";

                var sn = xl.Descendants(enoNode).FirstOrDefault();
                if (sn == null)
                    throw new Exception("Не найден элемент ServiceTypeCode");

                addpoplist.Add(new KeyValuePair<string, string>("ServiceTypeCode", sn.Value));
            }


            return xl.ToString();
        }

        private void btSendFromFile_Click(object sender, EventArgs e)
        {
            var files = Dialogs.FileBrowser(
            Owner: this,
            Title: "Выбор файлов с XML сообщений",
            Multiselect: true,
            CheckFileExists: true);

            foreach (var file in files)
                try
                {
                    tbMessageID.Text = null;
                    tbPutDate.Text = null;
                    tbBodyMessage.Text = File.ReadAllText(file);
                    FillAddProp(tbBodyMessage.Text);

                    MqSettingGeneric sets = CreateMqSetting();

                    tbMessageID.Text = mqBL.Send(
                        sets,
                        tbBodyMessage.Text.GetNullIfIsNullOrWhiteSpace(),
                        addpoplist.ToDictionary(x => x.Key, x => x.Value)
                        );

                    Config.Instance.MqSets =
                ArcasSetting.Instance.MqSets = sets;

                    ArcasSetting.Instance.Save();
                    Config.Instance.Save();

                }
                catch (Exception ex)
                {
                    Dialogs.ErrorF(this, ex.Expand());
                }
        }

        private void tbBodyMessage_TextChanged(object sender, EventArgs e)
        {
            try
            {
                FillAddProp(tbBodyMessage.Text);
            }
            catch { }
        }
    }
}
