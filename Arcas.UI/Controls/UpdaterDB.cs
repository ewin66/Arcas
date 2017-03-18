using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Arcas.BL;
using Arcas.Settings;
using Cav;
using Cav.Tfs;

namespace Arcas.Controls
{
    public partial class UpdaterDB : TabControlBase
    {
        public UpdaterDB()
        {
            InitializeComponent();
            this.Text = "Накатка БД";
        }

        Boolean textChanged = true;

        private void btSaveScript_Click(object sender, EventArgs e)
        {

            if (!textChanged)
                if (Dialogs.QuestionOKCancelF(this, "Текст скрипта не изменился с предыдущего запуска. Повторить?"))
                    return;

            btSaveScript.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            var savbl = new TfsDBSaveBL();
            savbl.StatusMessages += savbl_StatusMessages;
            var msg = savbl.SaveScript(
                (TfsDbLink)cbxTfsDbLinc.SelectedItem,
                tbScriptBody.Text,
                tbComment.Text,
                chbTransaction.Checked,
                lbLinkedWirkItem.Items.Cast<Lwi>().Select(x => x.ID).ToList());
            if (msg.IsNullOrWhiteSpace())
                msg = "Успешно";
            MessageBox.Show(this, msg);

            textChanged = false;
            btSaveScript.Enabled = true;
            savbl_StatusMessages(String.Empty);
            Config.Instance.SelestedTFSDB = cbxTfsDbLinc.Text;
            Cursor.Current = Cursors.Default;
        }

        private void savbl_StatusMessages(string Message)
        {
            this.SetSateProgress(Message);
        }

        private void tbxes_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
        }

        public override void RefreshTab()
        {
            #region заполняем cbxTfsBbLinc

            var SelName = Config.Instance.SelestedTFSDB;
            cbxTfsDbLinc.DataSource = Config.Instance.TfsDbLinks;
            cbxTfsDbLinc.SelectedItem = ((List<TfsDbLink>)cbxTfsDbLinc.DataSource).FirstOrDefault(x => x.Name == SelName);
            if (cbxTfsDbLinc.SelectedItem == null && cbxTfsDbLinc.Items.Count > 0)
                cbxTfsDbLinc.SelectedIndex = 0;

            bttvQueryRefresh.Enabled = cbxTfsDbLinc.Items.Count != 0;

            bttvQueryRefresh_Click(null, null);
            #endregion

            base.RefreshTab();
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            tbComment.Text = null;
            tbScriptBody.Text = null;
        }

        private void bttvQueryRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                tvQuerys.Nodes.Clear();

                TfsDbLink curset = cbxTfsDbLinc.SelectedItem as TfsDbLink;

                if (curset == null || curset.TFS.Server.IsNullOrWhiteSpace() || curset.TFS.Path.IsNullOrWhiteSpace())
                {
                    bttvQueryRefresh.Enabled = false;
                    return;
                }

                var tfs = new WrapTfs();
                var qs = tfs.QueryItemsGet(new Uri(curset.TFS.Server), curset.TFS.Path);

                Action<QueryItemNode, TreeNode> recNod = null;
                recNod = new Action<QueryItemNode, TreeNode>((qn, tn) =>
                    {
                        tn.Text = qn.Name;
                        tn.Tag = qn;
                        tn.StateImageIndex = 0;
                        if (!qn.IsFolder)
                            tn.StateImageIndex = 1;

                        foreach (var cqin in qn.ChildNodes)
                        {
                            var tnod = new TreeNode();
                            recNod(cqin, tnod);
                            tn.Nodes.Add(tnod);
                        }

                    });

                foreach (var qin in qs)
                {
                    var tnod = new TreeNode();
                    recNod(qin, tnod);
                    tvQuerys.Nodes.Add(tnod);
                }

                if (tvQuerys.Nodes.Count == 1)
                    tvQuerys.Nodes[0].Expand();
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
            }
        }

        private void tvQuerys_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                lbWorkItems.Items.Clear();

                TfsDbLink curset = cbxTfsDbLinc.SelectedItem as TfsDbLink;
                if (curset == null)
                    return;

                if (curset.TFS.Server.IsNullOrWhiteSpace())
                {
                    Dialogs.InformationF(this, "В настройках связки не указан сервер TFS");
                    return;
                }

                QueryItemNode qin = (QueryItemNode)e.Node.Tag;

                if (qin.IsFolder)
                    return;

                var tfs = new WrapTfs();
                var wims = tfs.WorkItemsFromQueryGet(new Uri(curset.TFS.Server), qin);
                foreach (var wi in wims)
                    lbWorkItems.Items.Add(new Lwi(wi));
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
            }

        }

        class Lwi
        {
            public Lwi(WorkItem wi)
            {
                ID = wi.ID;
                Title = wi.Title;
            }
            public int ID { get; set; }
            public String Title { get; set; }
            public override string ToString()
            {
                return $"({ID}) {Title}";
            }

        }

        private void btAddWorkItem_Click(object sender, EventArgs e)
        {
            foreach (Lwi item in lbWorkItems.SelectedItems)
            {
                Lwi si = (Lwi)item;

                if (lbLinkedWirkItem.Items.Cast<Lwi>().Any(x => x.ID == si.ID))
                    continue;
                lbLinkedWirkItem.Items.Add(si);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.KeyChar);
        }

        private void btAddInIDTask_Click(object sender, EventArgs e)
        {
            try
            {
                int idTask;
                if (!int.TryParse(tbIdTask.Text, out idTask))
                    return;

                TfsDbLink curset = cbxTfsDbLinc.SelectedItem as TfsDbLink;
                if (curset == null)
                    return;

                if (curset.TFS.Server.IsNullOrWhiteSpace())
                {
                    Dialogs.InformationF(this, "В настройках связки не указан сервер TFS");
                    return;
                }

                var tfs = new WrapTfs();
                var wi = tfs.WorkItemByIdGet(new Uri(curset.TFS.Server), idTask);
                if (lbLinkedWirkItem.Items.Cast<Lwi>().Any(x => x.ID == wi.ID))
                    return;
                lbLinkedWirkItem.Items.Add(new Lwi(wi));
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
            }
        }

        private void lbLinkedWirkItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
                return;
            foreach (var item in lbLinkedWirkItem.SelectedItems.Cast<Object>().ToArray())
                lbLinkedWirkItem.Items.Remove(item);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var item in lbLinkedWirkItem.SelectedItems.Cast<Object>().ToArray())
                lbLinkedWirkItem.Items.Remove(item);
        }

        private void lbLinkedWirkItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            if (lbLinkedWirkItem.SelectedItems.Count == 0)
                return;
            contextMenuStrip1.Show(Cursor.Position);
        }
    }
}
