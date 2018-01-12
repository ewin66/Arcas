using System.Linq;
using Cav;

namespace Arcas.Controls
{
    public partial class WsdlXsdCsGen : TabControlBase
    {
        public WsdlXsdCsGen()
        {
            InitializeComponent();
            this.Text = "Генерация C# из WSDL и XSD";



        }

        private void btFromFile_Click(object sender, System.EventArgs e)
        {
            var pathfile = Dialogs.FileBrowser(
                 Owner: this,
                 Title: "Файл с WSDL",
                 Filter: "*.wsdl|*.wsdl",
                 RestoreDirectory: true).FirstOrDefault();

            tbWsdlUri.Text = pathfile;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {

        }
    }
}
