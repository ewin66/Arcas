using System;
using System.Linq;
using System.Windows.Forms;
using Cav;

namespace Arcas.Settings
{
    public partial class TFSDBLinkForm : Form
    {
        public TFSDBLinkForm()
        {
            InitializeComponent();
            dgvTFSDB.DataSource = link;
        }

        private TFSDBList link = ArcasSettings.Settings.TfsDbLinks ?? Config.Instance.TfsDbLinks;

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

            var tb = new TFSBrowser();
            if (tb.ShowDialog(this) == DialogResult.OK)
                nl.TFS = tb.TFS;

            var cf = new ConnectionForm();
            if (cf.ShowDialog(this) == DialogResult.OK)
                nl.DB.ConnectionString = cf.ConnectionString;

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
            ArcasSettings.Settings.TfsDbLinks = link;
            ArcasSettings.Save();

            Config.Instance.TfsDbLinks = link;
            Config.Instance.Save();
        }
    }
}
