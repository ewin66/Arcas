using System;
using System.Windows.Forms;
using Arcas;
using Cav;
using Cav.Tfs;

namespace Arcas.Settings
{
    public partial class TFSBrowser : Form
    {
        public TFSBrowser()
        {
            InitializeComponent();
        }

        private WrapTfs tfsbl = new WrapTfs();

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
            var servtfs = tfsbl.ShowTeamProjectPicker(this);
            if (servtfs == null)
                return;
            tbServerTFS.Text = servtfs.ToString();
            TFS.Server = tbServerTFS.Text;
        }

        private void btSelPath_Click(object sender, EventArgs e)
        {
            try
            {
                var vc = tfsbl.VersionControlServerGet(new Uri(tbServerTFS.Text));
                tbPath.Text = tfsbl.ShowDialogChooseServerFolder(this, vc, tbPath.Text);
                TFS.Path = tbPath.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Expand());
            }
        }
    }
}
