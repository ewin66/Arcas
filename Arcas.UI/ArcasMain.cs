using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Arcas.Controls;
using Arcas.Settings;
using Cav;
using Cav.Container;

namespace Arcas
{
    public partial class ArcasMain : Form
    {
        public ArcasMain()
        {
            InitializeComponent();

            foreach (var tb in Locator.GetInstances<TabControlBase>())
            {
                var ts = new TabPage();

                ts.Controls.Add(tb);
                ts.Name = tb.Name;
                ts.Text = tb.Text;
                ts.UseVisualStyleBackColor = true;
                refreshTabAction.Add(tb.RefreshTab);
                tb.StateProgress += savbl_StatusMessages;
                tb.Dock = DockStyle.Fill;
                tcTabs.TabPages.Add(ts);
            }

            tabPageDBVer_Enter(null, null);
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

        private void ArcasMainMindow_FormClosing(object sender, FormClosingEventArgs e)
        {          
            Config.Instance.Save();
        }
    }
}
