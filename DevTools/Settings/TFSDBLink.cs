using System;
using System.Linq;
using System.Windows.Forms;
using Ppr;

namespace DevTools.Settings
{
    public partial class TFSDBLinkForm : Form
    {
        public TFSDBLinkForm()
        {
            InitializeComponent();
            dgvTFSDB.DataSource = link;
        }

        private TFSDBList link = DevToolsSettings.Settings.TfsDbLinks;

        #region Должно быть в общей библиотеке. Но тут таковых нема...

        /// <summary>
        /// Диалоговое окно ввода, ожидает ввода текста или нажатия кнопки и возвращает строку, являющуюся содержимым текстового поля.
        /// Если была нажата кнопка Отмена, то возвращается null.
        /// </summary>
        /// <param name="Prompt">Выражение, отображаемое в диалоговом окне в виде сообщения.</param>
        /// <param name="Title">Выражение, отображаемое в строке заголовка диалогового окна. </param>
        /// <param name="DefaulResponse">Выражение, отображаемое в качестве ответного сообщения по умолчанию, если ничего другого не было введено.</param>
        /// <param name="XPos">Числовое выражение, которое задает расстояние в пикселях между левым краем диалогового окна и левым краем экрана.
        ///  Если параметр XPos опущен, то диалоговое окно центрируется по горизонтали. </param>
        /// <param name="YPos">Числовое выражение, которое задает расстояние в пикселях между верхним краем диалогового окна и верхним краем экрана.
        ///  Если параметр YPos опущен, то диалоговое окно располагается на уровне, составляющем примерно треть высоты экрана. </param>
        /// <returns>Возвращает строку, являющуюся содержимым текстового поля.</returns>
        public static String InputBox(
            String Prompt,
            String Title = " ",
            String DefaulResponse = "")
        {
            // TODO Переделать InputBox
            // пока так
            // Надо переделать на генерацию формы...
            return Microsoft.VisualBasic.Interaction.InputBox(Prompt, Title, DefaulResponse);
        }

        #endregion


        private void btAdd_Click(object sender, EventArgs e)
        {
            var nl = new TfsDbLink();

            do
            {
                nl.Name = InputBox("Наименование связки TFS-DB.", "Наименование связки", "Новая связка");

                if (nl.Name.IsNullOrWhiteSpace())
                {
                    MessageBox.Show(this, "Не указано наименование связки");
                    return;
                }

                if (link.Any(x => x.Name == nl.Name))
                    MessageBox.Show(this, "Наименование должно быть уникальным");

            } while (link.Any(x => x.Name == nl.Name));

            var tb = new TFSBrowser();
            if (tb.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                nl.TFS = tb.TFS;

            var cf = new ConnectionForm();
            if (cf.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
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
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            if (dgvTFSDB.SelectedRows.Count == 0)
                return;
            var tdbli = (TfsDbLink)((DataGridViewRow)dgvTFSDB.SelectedRows[0]).DataBoundItem;

            if (e.ColumnIndex == 0)
            {
                var nn = InputBox("Наименование связки TFS-DB.", "Наименование связки", tdbli.Name);
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
                if (tb.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    tdbli.TFS = tb.TFS;
            }

            if (e.ColumnIndex == 2)
            {
                var cf = new ConnectionForm();
                cf.ConnectionString = tdbli.DB.ConnectionString;
                if (cf.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    tdbli.DB.ConnectionString = cf.ConnectionString;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            DevToolsSettings.Settings.TfsDbLinks = link;
            DevToolsSettings.Save();
        }
    }
}
