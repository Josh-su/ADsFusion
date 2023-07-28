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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Reflection.Emit;

namespace ADsFusion
{
    public partial class ServerAndAdminLogin : Form
    {
        private Settings _settings;

        public ServerAndAdminLogin()
        {
            InitializeComponent();

            _settings = new Settings();
        }

        private void ServerAndAdminLogin_Load(object sender, EventArgs e)
        {
            // Retrieved the saved credentials and informations from the app.config file an fill the appropriate textboxes with the saved credentials
            txtbDomain1.Text = Properties.Settings.Default.Domain1;
            txtbDomain2.Text = Properties.Settings.Default.Domain2;
            txtbUsername1.Text = Properties.Settings.Default.Username1;
            txtbUsername2.Text = Properties.Settings.Default.Username2;
            txtbPassword1.Text = Properties.Settings.Default.Password1;
            txtbPassword2.Text = Properties.Settings.Default.Password2;
            txtbGroup1.Text = Properties.Settings.Default.Group1;
            txtbGroup2.Text = Properties.Settings.Default.Group2;
            txtbOU1.Text = Properties.Settings.Default.OU1;
            txtbOU2.Text = Properties.Settings.Default.OU2;
        }

        /// <summary>
        /// Login button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            List<string> server1Informations = new List<string>
            {
                txtbDomain1.Text,
                txtbUsername1.Text,
                txtbPassword1.Text,
                txtbGroup1.Text
            };
            List<string> server2Informations = new List<string>
            {
                txtbDomain2.Text,
                txtbUsername2.Text,
                txtbPassword2.Text,
                txtbGroup2.Text
            };

            int server1NotEmptyInformations = 0;
            int server2NotEmptyInformations = 0;

            bool domain1Success = false;
            bool domain2Success = false;

            foreach (string information1 in server1Informations)
            {
                if (!string.IsNullOrEmpty(information1))
                {
                    server1NotEmptyInformations++;
                }
            }
            foreach (string information2 in server2Informations)
            {
                if (!string.IsNullOrEmpty(information2))
                {
                    server2NotEmptyInformations++;
                }
            }

            if (server1NotEmptyInformations > 0 && server1NotEmptyInformations < server1Informations.Count)
            {
                MessageBox.Show("Veuillez écrire toute les informations pour le serveur 1");
            }
            else if (server1NotEmptyInformations == server1Informations.Count)
            {
                domain1Success = LoginDomain(txtbDomain1.Text, txtbUsername1.Text, txtbPassword1.Text, txtbGroup1.Text);
            }
            if (server2NotEmptyInformations > 0 && server2NotEmptyInformations < server2Informations.Count)
            {
                MessageBox.Show("Veuillez écrire toute les informations pour le serveur 2");
            }
            else if (server2NotEmptyInformations == server2Informations.Count)
            {
                domain2Success = LoginDomain(txtbDomain2.Text, txtbUsername2.Text, txtbPassword2.Text, txtbGroup2.Text);
            }

            if (domain1Success && domain2Success)
            {
                // Check if at least one domain's login is successful before saving credentials
                if (domain1Success || domain2Success)
                {
                    if (domain1Success && !domain2Success)
                    {
                        SaveCredentials(txtbDomain1.Text, null, txtbUsername1.Text, null, txtbPassword1.Text, null, txtbGroup1.Text, null, txtbOU1.Text, null);
                    }

                    if (domain2Success || !domain1Success)
                    {
                        SaveCredentials(null, txtbDomain2.Text, null, txtbUsername2.Text, null, txtbPassword2.Text, null, txtbGroup2.Text, null, txtbOU2.Text);
                    }

                    if (domain1Success && domain2Success)
                    {
                        SaveCredentials(txtbDomain1.Text, txtbDomain2.Text, txtbUsername1.Text, txtbUsername2.Text, txtbPassword1.Text, txtbPassword2.Text, txtbGroup1.Text, txtbGroup2.Text, txtbOU1.Text, txtbOU2.Text);
                    }
                }
            }
            _settings.ShowDialog();
        }

        // Login for Domain1
        private bool LoginDomain(string domain, string username, string password, string groupName)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain))
                {
                    bool isAuthenticated = context.ValidateCredentials(username, password);

                    if (isAuthenticated)
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);

                        if (user.IsMemberOf(context, IdentityType.Name, groupName))
                        {
                            MessageBox.Show($"Login successful for {domain} domain as an administrator!");
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("You are not authorized to login to this application.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Invalid username or password for {domain} domain.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

            return false;
        }

        private void SaveCredentials(string domain1, string domain2, string username1, string username2, string password1, string password2, string group1, string group2, string ou1, string ou2)
        {
            // Update the user settings with the provided credentials
            Properties.Settings.Default.Domain1 = domain1;
            Properties.Settings.Default.Domain2 = domain2;
            Properties.Settings.Default.Username1 = username1;
            Properties.Settings.Default.Username2 = username2;
            Properties.Settings.Default.Password1 = password1;
            Properties.Settings.Default.Password2 = password2;
            Properties.Settings.Default.Group1 = group1;
            Properties.Settings.Default.Group2 = group2;
            Properties.Settings.Default.OU1 = ou1;
            Properties.Settings.Default.OU2 = ou2;

            // Save the changes
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Logout button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear all the saved information by resetting the user settings
            Properties.Settings.Default.Reset();

            // Clear all textboxes on the form and its nested containers
            ClearAllTextBoxes();
        }

        /// <summary>
        /// Clear all textboxes on the form and its nested containers
        /// </summary>
        private void ClearAllTextBoxes()
        {
            foreach (System.Windows.Forms.Control c in this.Controls)
            {
                if (c is System.Windows.Forms.TextBox textBox)
                {
                    textBox.Clear();
                }
            }
        }
    }
}
