using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Arcas.BL;
using Cav;
using Cav.Tfs;

namespace Arcas.Settings
{
    public partial class TFSDBLinkForm : Form
    {
        public TFSDBLinkForm()
        {
            InitializeComponent();
            dgvTFSDB.DataSource = link;
        }

        private TFSDBList link = Config.Instance.TfsDbSets;

        private void btAdd_Click(object sender, EventArgs e)
        {
            var nl = new TfsDbLink();

            do
            {
                nl.Name = Dialogs.InputBox(this, "Наименование связки TFS-DB.", "Наименование связки", "Новая связка");

                if (nl.Name.IsNullOrWhiteSpace())
                {
                    MessageBox.Show(this, "Не указано наименование связки");
                    return;
                }

                if (link.Any(x => x.Name == nl.Name))
                    MessageBox.Show(this, "Наименование должно быть уникальным");

            } while (link.Any(x => x.Name == nl.Name));

            WrapTfs wrapTfs = new WrapTfs();
            nl.ServerUri = wrapTfs.ShowTeamProjectPicker(this);
            if (nl.ServerUri == null)
            {
                Dialogs.ErrorF(this, "Не выбран сервер(проект).");
                return;
            }

            var vc = wrapTfs.VersionControlServerGet(nl.ServerUri);

            var selItem = wrapTfs.ShowDialogChooseItem(this, vc);

            if (selItem.ItemType != ItemType.File)
            {
                Dialogs.ErrorF(this, "Необходимо выбрать файл настроек");
            }

            var tempFile = Path.Combine(DomainContext.TempPath, Guid.NewGuid().ToString());

            wrapTfs.VersionControlServerDownloadFile(vc, selItem.Path, tempFile);

            var encr = new DeEncryp();
            var sets = encr.Decript(tempFile, selItem.Path);

            if (sets == null)
            {
                Dialogs.InformationF(this, "Файл настроек не расшифрован. Либо выбран не файл настроек, либо ");
                return;
            }

            link.Add(nl);

            dgvTFSDB.Update();
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (dgvTFSDB.SelectedRows.Count == 0)
                return;
            foreach (DataGridViewRow item in dgvTFSDB.SelectedRows)
            {
                if (item.DataBoundItem == null)
                    continue;

                link.Remove((TfsDbLink)item.DataBoundItem);
            }
        }

        private void dgvTFSDB_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (e.Button != MouseButtons.Left)
                return;
            if (dgvTFSDB.SelectedRows.Count == 0)
                return;
            var tdbli = (TfsDbLink)((DataGridViewRow)dgvTFSDB.SelectedRows[0]).DataBoundItem;

            if (e.ColumnIndex == 0)
            {
                var nn = Dialogs.InputBox(this, "Наименование связки TFS-DB.", "Наименование связки", tdbli.Name);
                if (nn.IsNullOrWhiteSpace())
                {
                    MessageBox.Show(this, "Не указано наименование связки");
                    return;
                }
                tdbli.Name = nn;
            }

            if (e.ColumnIndex == 1)
            {
                var tb = new TFSBrowser();
                tb.TFS = tdbli.TFS;
                if (tb.ShowDialog(this) == DialogResult.OK)
                    tdbli.TFS = tb.TFS;
            }

            if (e.ColumnIndex == 2)
            {
                var cf = new ConnectionForm();
                cf.ConnectionString = tdbli.DB.ConnectionString;
                if (cf.ShowDialog(this) == DialogResult.OK)
                    tdbli.DB.ConnectionString = cf.ConnectionString;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Config.Instance.TfsDbLinks = link;
            Config.Instance.Save();
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            String setName = Dialogs.InputBox(this,





        }
    }
}
