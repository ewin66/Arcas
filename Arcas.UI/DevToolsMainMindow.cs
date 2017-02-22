using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Cav;
using Arcas.Controls;
using Arcas.Settings;


namespace Arcas
{
    public partial class ArcasMainMindow : Form
    {
        public ArcasMainMindow()
        {
            InitializeComponent();

            var tabsTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetExportedTypes()).Where(x => x.IsSubclassOf(typeof(TabControlBase))).ToList();

            foreach (var tabType in tabsTypes)
            {
                TabControlBase tb = (TabControlBase)Activator.CreateInstance(tabType);
                var ts = new TabPage();

                ts.Controls.Add(tb);
                ts.Name = tb.Name;
                ts.Text = tb.Text;
                ts.UseVisualStyleBackColor = true;
                refreshTabAction.Add(tb.RefreshTab);
                tb.StateProgress += savbl_StatusMessages;
                tb.Dock = DockStyle.Fill;
                ts.TabIndex = tabsTypes.IndexOf(tabType) + 1;
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
            ArcasSetting.Instance.Save();
        }
    }
}
