using System;
using System.Linq;
using Arcas.BL;
using Cav;

namespace Arcas.Controls
{
    public partial class WsdlXsdCsGen : TabControlBase
    {
        public WsdlXsdCsGen(
            CsGenFromWsdlXsd csGenFromWsdlXsd)
        {
            InitializeComponent();
            this.Text = "Генерация C# из WSDL и XSD";

            this.csGenFromWsdlXsd = csGenFromWsdlXsd;


            tbWsdlUri.Text = Config.Instance.WsdlXsdGenSetting.Wsdl_PathToWsdl;
            tbSaveTo.Text = Config.Instance.WsdlXsdGenSetting.Wsdl_PathToSaveFile;
            tbTargetNamespace.Text = Config.Instance.WsdlXsdGenSetting.Wsdl_Namespace;
        }

        private CsGenFromWsdlXsd csGenFromWsdlXsd;

        private void btFromFile_Click(object sender, System.EventArgs e)
        {
            var pathfile = Dialogs.FileBrowser(
                 Owner: this,
                 Title: "Файл с WSDL",
                 Filter: "*.wsdl|*.wsdl",
                 RestoreDirectory: true).FirstOrDefault();

            tbWsdlUri.Text = pathfile;
        }

        private void btSelFileForSave_Click(object sender, System.EventArgs e)
        {
            var pathfile = Dialogs.SaveFile(
            Owner: this,
            Title: "Сохранить код в файл",
            Filter: "C# файл| *.cs",
            FileName: tbSaveTo.Text.GetNullIfIsNullOrWhiteSpace(),
            DefaultExt: "cs",
            RestoreDirectory: true);

            tbSaveTo.Text = pathfile.GetNullIfIsNullOrWhiteSpace();
        }

        private WsdlXsdGenSettingT CreateSetting()
        {
            return new WsdlXsdGenSettingT()
            {
                Wsdl_PathToWsdl = tbWsdlUri.Text,
                Wsdl_PathToSaveFile = tbSaveTo.Text,
                Wsdl_Namespace = tbTargetNamespace.Text
            };
        }

        private void btGenerateCsFromWsdl_Click(object sender, System.EventArgs e)
        {
            try
            {

                Config.Instance.WsdlXsdGenSetting = CreateSetting();
                Config.Instance.Save();

                var msg = csGenFromWsdlXsd.GenFromWsdl(
                    uriWsdl: tbWsdlUri.Text,
                    createAsync: chbCreateAsuncMethod.Checked,
                    targetNamespace: tbTargetNamespace.Text,
                    outputFile: tbSaveTo.Text,
                    generateClient: rbGenClient.Checked);

                if (!msg.IsNullOrWhiteSpace())
                    Dialogs.ErrorF(this, msg);
                else
                    Dialogs.InformationF(this, "Готово");
            }
            catch (Exception ex)
            {
                Dialogs.ErrorF(this, ex.Expand());
            }
        }
    }
}
