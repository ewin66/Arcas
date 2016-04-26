using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevTools.Controls;
using DevTools.Settings;
using Ppr;


namespace DevTools
{
    public partial class DevToolsMainMindow : Form
    {
        public DevToolsMainMindow()
        {
            InitializeComponent();

            foreach (TabPage tab in tcTabs.TabPages)
                foreach (Control cntl in tab.Controls)
                    if (cntl is TabControlBase)
                    {
                        refreshTabAction.Add((cntl as TabControlBase).RefreshTab);
                        (cntl as TabControlBase).StateProgress += savbl_StatusMessages;
                    }
        }

        private List<Action> refreshTabAction = new List<Action>();

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TFSDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new TFSDBLinkForm()).ShowDialog(this);
            tabPageDBVer_Enter(null, null);
        }

        private void tabPageDBVer_Enter(object sender, EventArgs e)
        {
            foreach (var item in refreshTabAction)
                try
                {
                    item.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Expand(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

        }

        void savbl_StatusMessages(string Message)
        {
            tsStatusLabel.Text = Message;
            this.Refresh();
        }

        private void DevToolsMainMindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
