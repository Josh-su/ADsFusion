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

namespace ADsFusion
{
    public partial class ServerAndAdminLogin : Form
    {
        private bool server1Test;
        private bool server2Test;

        public ServerAndAdminLogin()
        {
            InitializeComponent();
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
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            TestCredentials(txtbDomain1.Text, txtbDomain2.Text, txtbUsername1.Text, txtbUsername2.Text, txtbPassword1.Text, txtbPassword2.Text, txtbGroup1.Text, txtbGroup2.Text);
            if (server1Test && server2Test)
            {
                SaveCredentials(txtbDomain1.Text, txtbDomain2.Text, txtbUsername1.Text, txtbUsername2.Text, txtbPassword1.Text, txtbPassword2.Text, txtbGroup1.Text, txtbGroup2.Text);
            }
        }

        private void TestCredentials(string domain1, string domain2, string username1, string username2, string password1, string password2, string group1, string group2)
        {
            try
            {
                // Create a PrincipalContext object for the specified domain
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain1))
                {
                    // Authenticate the user credentials against the domain
                    bool isAuthenticated = context.ValidateCredentials(username1, password1);

                    if (isAuthenticated)
                    {
                        // Get the user's Principal object
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username1);

                        // Check if the user is a member of the administrators group
                        if (user.IsMemberOf(context, IdentityType.Name, group1))
                        {
                            // If the user is an administrator, show a message box indicating success
                            MessageBox.Show("Login successful for 'ceol.gyre.ch' domain as an administrator!");
                            server1Test = true;
                        }
                        else
                        {
                            // If the user is not an administrator, show an error message
                            MessageBox.Show("You are not authorized to login to this application.");
                            server1Test = false;
                        }
                    }
                    else
                    {
                        // If the credentials are not valid, show an error message
                        MessageBox.Show("Invalid username or password for 'ceol.gyre.ch' domain.");
                        server1Test = false;
                    }
                }

                // Create a PrincipalContext object for the specified domain
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain2))
                {
                    // Authenticate the user credentials against the domain
                    bool isAuthenticated = context.ValidateCredentials(username2, password2);

                    if (isAuthenticated)
                    {
                        // Get the user's Principal object
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username2);

                        // Check if the user is a member of the administrators group
                        if (user.IsMemberOf(context, IdentityType.Name, group2))
                        {
                            // If the user is an administrator, show a message box indicating success
                            MessageBox.Show("Login successful for 'dgep.edu-vaud.ch' domain as an administrator!");
                            server2Test = true;
                        }
                        else
                        {
                            // If the user is not an administrator, show an error message
                            MessageBox.Show("You are not authorized to login to this application.");
                            server2Test = false;
                        }
                    }
                    else
                    {
                        // If the credentials are not valid, show an error message
                        MessageBox.Show("Invalid username or password for 'dgep.edu-vaud.ch' domain.");
                        server2Test = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // If an error occurs during authentication, show an error message
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void SaveCredentials(string domain1, string domain2, string username1, string username2, string password1, string password2, string group1, string group2)
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

            // Save the changes
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // Clear all the saved information by resetting the user settings
            Properties.Settings.Default.Reset();

            // Clear all textboxes on the form and its nested containers
            ClearAllTextBoxes();
        }

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
