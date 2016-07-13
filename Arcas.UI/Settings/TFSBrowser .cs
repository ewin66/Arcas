using System;
using System.Windows.Forms;
using DevTools.BL.TFS;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Controls.Common;
using Cav;

namespace DevTools.Settings
{
    public partial class TFSBrowser : Form
    {
        public TFSBrowser()
        {
            InitializeComponent();
        }

        private TFSRoutineBL tfsbl = new TFSRoutineBL();

        private TfsSetting tfsSetting = null;
        public TfsSetting TFS
        {
            get
            {
                if (tfsSetting == null)
                    tfsSetting = new TfsSetting();
                return tfsSetting;
            }

            set
            {
                tbPath.Text = null;
                tbServerTFS.Text = null;

                tfsSetting = value;
                if (tfsSetting == null)
                    return;

                tbServerTFS.Text = tfsSetting.Server;
                tbPath.Text = tfsSetting.Path;
            }
        }

        private void btSelProject_Click(object sender, EventArgs e)
        {
            TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.NoProject, false);
            if (tpp.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                return;

            tbServerTFS.Text = tpp.SelectedTeamProjectCollection.Uri.ToString();
            TFS.Server = tbServerTFS.Text;
        }

        private void btSelPath_Click(object sender, EventArgs e)
        {
            try
            {
                DialogChooseServerFolder dcf = new DialogChooseServerFolder(tfsbl.VersionControl(tbServerTFS.Text), tbPath.Text);
                dcf.ShowInTaskbar = false;
                if (dcf.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                    return;
                tbPath.Text = dcf.CurrentServerItem;
                TFS.Path = tbPath.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Expand());
            }
        }
    }
}
