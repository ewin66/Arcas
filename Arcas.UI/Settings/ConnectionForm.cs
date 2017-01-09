using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Cav;

namespace DevTools.Settings
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();

            tbLogin.Enabled = !chbWindowsAuth.Checked;
            tbPass.Enabled = !chbWindowsAuth.Checked;
        }

        public String ConnectionString
        {
            get
            {
                if (scsb == null)
                    return null;
                return scsb.ToString();
            }
            set
            {
                scsb = new SqlConnectionStringBuilder(value);

                tbDBName.Text = scsb.InitialCatalog;
                tbServer.Text = scsb.DataSource;
                tbLogin.Text = scsb.UserID;
                tbPass.Text = scsb.Password;
                chbWindowsAuth.Checked = scsb.IntegratedSecurity;
                chbWindowsAuth_CheckedChanged(null, null);
            }
        }

        private SqlConnectionStringBuilder scsb = null;

        private void chbWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            tbLogin.Enabled = !chbWindowsAuth.Checked;
            tbPass.Enabled = !chbWindowsAuth.Checked;
        }

        private void btTestConnection_Click(object sender, EventArgs e)
        {

            String messg = null;
            try
            {
                ConnectionString = DomainContext.InitConnection(
                dbName: tbDBName.Text,
                server: tbServer.Text,
                login: tbLogin.Text,
                pass: tbPass.Text,
                MARS: false,
                integratedSecurity: chbWindowsAuth.Checked,
                applicationName: "DevTools " + Environment.MachineName + @":" + Environment.UserDomainName + @"\" + Environment.UserName);

                btOk.Enabled = true;
            }
            catch (Exception ex)
            {
                messg = ex.Expand();

                btOk.Enabled = false;
            }

            if (String.IsNullOrWhiteSpace(messg))
                messg = "Ok";
            MessageBox.Show(this, messg, "Тест соединения");
        }
    }
}
