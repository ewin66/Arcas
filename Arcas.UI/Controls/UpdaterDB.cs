using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevTools.BL;
using Cav;

namespace DevTools.Controls
{
    public partial class UpdaterDB : TabControlBase
    {
        public UpdaterDB()
        {
            InitializeComponent();
            try
            {
                cbxTfsDbLinc.Text = ArcasSetting.Instance.SelestedTFSDB;
            }
            catch { }
        }

        Boolean textChanged = true;

        private void btSaveScript_Click(object sender, EventArgs e)
        {

            if (!textChanged)
                if (MessageBox.Show(this, "Текст скрипта не изменился с предыдущего запуска. Повторить?", "DevTools", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;

            btSaveScript.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            var savbl = new TfsDBSaveBL();
            savbl.StatusMessages += savbl_StatusMessages;
            var msg = savbl.SaveScript((TfsDbLink)cbxTfsDbLinc.SelectedItem, tbScriptBody.Text, tbComment.Text, chbTransaction.Checked);
            if (msg.IsNullOrWhiteSpace())
                msg = "Успешно";
            MessageBox.Show(this, msg);

            textChanged = false;
            btSaveScript.Enabled = true;
            savbl_StatusMessages(String.Empty);
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

            var SelName = ArcasSetting.Instance.SelestedTFSDB;
            cbxTfsDbLinc.DataSource = DevToolsSettings.Settings.TfsDbLinks;
            cbxTfsDbLinc.SelectedItem = ((List<TfsDbLink>)cbxTfsDbLinc.DataSource).FirstOrDefault(x => x.Name == SelName);
            if (cbxTfsDbLinc.SelectedItem == null && cbxTfsDbLinc.Items.Count > 0)
                cbxTfsDbLinc.SelectedIndex = 0;

            #endregion

            base.RefreshTab();
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            tbComment.Text = null;
            tbScriptBody.Text = null;
        }
    }
}
