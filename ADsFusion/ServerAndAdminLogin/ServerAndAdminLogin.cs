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
        private int _initialHeight;

        private List<System.Windows.Forms.TextBox> dynamicTextBoxesOu1 = new List<System.Windows.Forms.TextBox>();
        private List<System.Windows.Forms.TextBox> dynamicTextBoxesOu2 = new List<System.Windows.Forms.TextBox>();

        public ServerAndAdminLogin()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            _initialHeight = this.Height;
        }

        private void ServerAndAdminLogin_Load(object sender, EventArgs e)
        {
            //dynamicTextBoxesOu1.Add(txtbOU1); // Add the existing textbox to the dynamic list
            //dynamicTextBoxesOu2.Add(txtbOU2); // Add the existing textbox to the dynamic list

            // Retrieved the saved credentials and informations from the app.config file an fill the appropriate textboxes with the saved credentials
            txtbDomain1.Text = Properties.Settings.Default.Domain1;
            txtbDomain2.Text = Properties.Settings.Default.Domain2;
            txtbUsername1.Text = Properties.Settings.Default.Username1;
            txtbUsername2.Text = Properties.Settings.Default.Username2;
            txtbPassword1.Text = Properties.Settings.Default.Password1;
            txtbPassword2.Text = Properties.Settings.Default.Password2;
            txtbGroup1.Text = Properties.Settings.Default.Group1;
            txtbGroup2.Text = Properties.Settings.Default.Group2;

            // Load dynamic textbox data
            string[] ou1TextboxValues = Properties.Settings.Default.OU1.Split('|');
            for (int i = 0; i < ou1TextboxValues.Length - 1; i++) // Length - 1 to exclude the last empty entry
            {
                System.Windows.Forms.TextBox newTextbox = new System.Windows.Forms.TextBox();
                newTextbox.Size = txtbOU1.Size; // Set the size
                if (dynamicTextBoxesOu1.Count > 0)
                {
                    newTextbox.Location = new Point(txtbOU1.Left, dynamicTextBoxesOu1[dynamicTextBoxesOu1.Count - 1].Bottom + 10);
                }
                else
                {
                    newTextbox.Location = new Point(txtbOU1.Left, txtbOU1.Bottom - 20);
                }
                newTextbox.Text = ou1TextboxValues[i];
                dynamicTextBoxesOu1.Add(newTextbox);
                Controls.Add(newTextbox);
                AdjustFormLayout();
            }
            string[] ou2TextboxValues = Properties.Settings.Default.OU2.Split('|');
            for (int i = 0; i < ou2TextboxValues.Length - 1; i++) // Length - 1 to exclude the last empty entry
            {
                System.Windows.Forms.TextBox newTextbox = new System.Windows.Forms.TextBox();
                newTextbox.Size = txtbOU2.Size; // Set the size
                if (i == 0)
                {
                    newTextbox.Location = new Point(txtbOU2.Left, txtbOU2.Bottom - 20);
                }
                else
                {
                    newTextbox.Location = new Point(txtbOU2.Left, dynamicTextBoxesOu2[dynamicTextBoxesOu2.Count - 1].Bottom + 10);
                }
                newTextbox.Text = ou2TextboxValues[i];
                dynamicTextBoxesOu2.Add(newTextbox);
                Controls.Add(newTextbox);
                AdjustFormLayout();
            }
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
                        SaveCredentials(txtbDomain1.Text, null, txtbUsername1.Text, null, txtbPassword1.Text, null, txtbGroup1.Text, null);
                    }

                    if (domain2Success || !domain1Success)
                    {
                        SaveCredentials(null, txtbDomain2.Text, null, txtbUsername2.Text, null, txtbPassword2.Text, null, txtbGroup2.Text);
                    }

                    if (domain1Success && domain2Success)
                    {
                        SaveCredentials(txtbDomain1.Text, txtbDomain2.Text, txtbUsername1.Text, txtbUsername2.Text, txtbPassword1.Text, txtbPassword2.Text, txtbGroup1.Text, txtbGroup2.Text);
                    }
                }
            }
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

        private void SaveCredentials(string domain1, string domain2, string username1, string username2, string password1, string password2, string group1, string group2)
        {
            // Clear all the saved information by resetting the user settings
            Properties.Settings.Default.Reset();

            // Update the user settings with the provided credentials
            Properties.Settings.Default.Domain1 = domain1;
            Properties.Settings.Default.Domain2 = domain2;
            Properties.Settings.Default.Username1 = username1;
            Properties.Settings.Default.Username2 = username2;
            Properties.Settings.Default.Password1 = password1;
            Properties.Settings.Default.Password2 = password2;
            Properties.Settings.Default.Group1 = group1;
            Properties.Settings.Default.Group2 = group2;

            // Save dynamic textbox information
            StringBuilder dynamicTextboxesData = new StringBuilder();
            foreach (System.Windows.Forms.TextBox textBox in dynamicTextBoxesOu1)
            {
                if(textBox.Text.Length > 4)
                {
                    dynamicTextboxesData.Append(textBox.Text);
                    dynamicTextboxesData.Append("|"); // Use a separator between values
                }
            }
            Properties.Settings.Default.OU1 = dynamicTextboxesData.ToString();

            dynamicTextboxesData.Clear(); // Clear for next set of textboxes
            foreach (System.Windows.Forms.TextBox textBox in dynamicTextBoxesOu2)
            {
                if (textBox.Text.Length > 4)
                {
                    dynamicTextboxesData.Append(textBox.Text);
                    dynamicTextboxesData.Append("|"); // Use a separator between values
                }
            }
            Properties.Settings.Default.OU2 = dynamicTextboxesData.ToString();

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

            foreach (System.Windows.Forms.TextBox dynamicTextBox in dynamicTextBoxesOu1)
            {
                Controls.Remove(dynamicTextBox);
            }
            dynamicTextBoxesOu1.Clear();

            foreach (System.Windows.Forms.TextBox dynamicTextBox in dynamicTextBoxesOu2)
            {
                Controls.Remove(dynamicTextBox);
            }
            dynamicTextBoxesOu2.Clear();

            AdjustFormLayout();
        }

        #region Add & toggle OU textbox
        private void button1_Click(object sender, EventArgs e)
        {
            if (dynamicTextBoxesOu1.Count < 5)
            {
                System.Windows.Forms.TextBox newTextBox = new System.Windows.Forms.TextBox();
                newTextBox.Size = txtbOU1.Size; // Set the size
                if (dynamicTextBoxesOu1.Count > 0)
                {
                    newTextBox.Location = new Point(txtbOU1.Left, dynamicTextBoxesOu1[dynamicTextBoxesOu1.Count - 1].Bottom + 10);
                }
                else
                {
                    newTextBox.Location = new Point(txtbOU1.Left, txtbOU1.Bottom - 20);
                }
                dynamicTextBoxesOu1.Add(newTextBox);
                Controls.Add(newTextBox);

                AdjustFormLayout();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (dynamicTextBoxesOu1.Count > 0)
            {
                Controls.Remove(dynamicTextBoxesOu1[dynamicTextBoxesOu1.Count -1]);
                dynamicTextBoxesOu1.RemoveAt(dynamicTextBoxesOu1.Count - 1);

                AdjustFormLayout();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dynamicTextBoxesOu2.Count < 5)
            {
                System.Windows.Forms.TextBox newTextBox = new System.Windows.Forms.TextBox();
                newTextBox.Size = txtbOU2.Size; // Set the size
                if (dynamicTextBoxesOu2.Count > 0)
                {
                    newTextBox.Location = new Point(txtbOU2.Left, dynamicTextBoxesOu2[dynamicTextBoxesOu2.Count - 1].Bottom + 10);
                }
                else
                {
                    newTextBox.Location = new Point(txtbOU2.Left, txtbOU2.Bottom -20);
                }
                dynamicTextBoxesOu2.Add(newTextBox);
                Controls.Add(newTextBox);

                AdjustFormLayout();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dynamicTextBoxesOu2.Count > 0)
            {
                Controls.Remove(dynamicTextBoxesOu2[dynamicTextBoxesOu2.Count - 1]);
                dynamicTextBoxesOu2.RemoveAt(dynamicTextBoxesOu2.Count - 1);

                AdjustFormLayout();
            }
        }

        private void AdjustFormLayout()
        {
            int textboxNb = Math.Max(
                dynamicTextBoxesOu1.Count,
                dynamicTextBoxesOu2.Count
            );

            // Adjust the form's height to accommodate added textboxes
            this.Height =  _initialHeight + textboxNb * 30;
        }
        #endregion
    }
}
