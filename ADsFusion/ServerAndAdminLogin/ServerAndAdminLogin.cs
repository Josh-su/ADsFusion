using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;
using System.Configuration;
using System.Windows.Controls;

namespace ADsFusion
{
    public partial class ServerAndAdminLogin : Form
    {
        public ServerAndAdminLogin()
        {
            InitializeComponent();
        }

        private void ServerAndAdminLogin_Load(object sender, EventArgs e)
        {
            // Retrieved the saved credentials and informations from the app.config file an fill the appropriate textboxes with the saved credentials
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Domain1"]))
            {
                txtbDomain1.Text = ConfigurationManager.AppSettings["Domain1"];
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Domain2"]))
            {
                txtbDomain2.Text = ConfigurationManager.AppSettings["Domain2"];
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Username1"]))
            {
                txtbUsername1.Text = ConfigurationManager.AppSettings["Username1"];
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Username2"]))
            {
                txtbUsername2.Text = ConfigurationManager.AppSettings["Username2"];
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Password1"]))
            {
                txtbPassword1.Text = ConfigurationManager.AppSettings["Password1"];
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Password2"]))
            {
                txtbPassword2.Text = ConfigurationManager.AppSettings["Password2"];
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Group1"]))
            {
                txtbGroup1.Text = ConfigurationManager.AppSettings["Group1"];
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Group2"]))
            {
                txtbGroup2.Text = ConfigurationManager.AppSettings["Group2"];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveCredentials(txtbDomain1.Text, txtbDomain2.Text, txtbUsername1.Text);
        }

        private void SaveCredentials(string domain1, string domain2, string username1)
        {
            // update the connection string in the app.config file
            ConfigurationManager.AppSettings["Domain1"] = domain1;
            ConfigurationManager.AppSettings["Domain2"] = domain2;
            ConfigurationManager.AppSettings["Username1"] = username1;
        }

        //logout
        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            ClearAllTextBoxes();
        }

        private void ClearAllTextBoxes( )
        {
            foreach (System.Windows.Forms.Control c in this.Controls)
            {
                if (c is System.Windows.Forms.TextBox textBox)
                {
                    textBox.Text = "";
                }
                else if (c.HasChildren)
                {
                    ClearAllTextBoxes(); // Recursively handle nested controls (e.g., panels, group boxes, etc.)
                }
            }
        }
    }
}
