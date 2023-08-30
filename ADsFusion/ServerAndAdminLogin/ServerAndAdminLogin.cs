﻿using System;
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

        private List<System.Windows.Forms.TextBox> _allTextBoxesGroup1 = new List<System.Windows.Forms.TextBox>();
        private List<System.Windows.Forms.TextBox> _allTextBoxesGroup2 = new List<System.Windows.Forms.TextBox>();

        StringBuilder dynamicTextboxesData = new StringBuilder();

        public ServerAndAdminLogin()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;

            _initialHeight = this.Height;
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
            List<string> groups1TextboxValues = Properties.Settings.Default.Groups1.Split('|').ToList();
            groups1TextboxValues.Remove(groups1TextboxValues.Last());
            txtbGroup2.Text = Properties.Settings.Default.Group2;
            List<string> groups2TextboxValues = Properties.Settings.Default.Groups2.Split('|').ToList();
            groups2TextboxValues.Remove(groups2TextboxValues.Last());

            _allTextBoxesGroup1.Add(txtbGroups1);
            _allTextBoxesGroup2.Add(txtbGroups2);

            // Load dynamic textbox data
            foreach (string value in groups1TextboxValues)
            {
                System.Windows.Forms.TextBox newTextbox = new System.Windows.Forms.TextBox();
                newTextbox.Size = txtbGroups1.Size; // Set the size
                if (_allTextBoxesGroup1.Count > 0)
                {
                    newTextbox.Location = new Point(txtbGroups1.Left, _allTextBoxesGroup1[_allTextBoxesGroup1.Count - 1].Bottom + 10);
                    newTextbox.Text = value;
                    _allTextBoxesGroup1.Add(newTextbox);
                    Controls.Add(newTextbox);
                    AdjustFormLayout();
                }
                else
                {
                    txtbGroups1.Text = value;
                }
            }
            foreach (string value in groups2TextboxValues)
            {
                System.Windows.Forms.TextBox newTextbox = new System.Windows.Forms.TextBox();
                newTextbox.Size = txtbGroups2.Size; // Set the size
                if (_allTextBoxesGroup1.Count > 0)
                {
                    newTextbox.Location = new Point(txtbGroups2.Left, _allTextBoxesGroup2[_allTextBoxesGroup2.Count - 1].Bottom + 10);
                    newTextbox.Text = value;
                    _allTextBoxesGroup2.Add(newTextbox);
                    Controls.Add(newTextbox);
                    AdjustFormLayout();
                }
                else
                {
                    txtbGroups2.Text = value;
                }
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
                txtbGroup1.Text,
                txtbGroups1.Text
            };
            List<string> server2Informations = new List<string>
            {
                txtbDomain2.Text,
                txtbUsername2.Text,
                txtbPassword2.Text,
                txtbGroup2.Text,
                txtbGroups2.Text
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
                        this.Close();
                    }

                    if (domain2Success && !domain1Success)
                    {
                        SaveCredentials(null, txtbDomain2.Text, null, txtbUsername2.Text, null, txtbPassword2.Text, null, txtbGroup2.Text);
                        this.Close();
                    }

                    if (domain1Success && domain2Success)
                    {
                        SaveCredentials(txtbDomain1.Text, txtbDomain2.Text, txtbUsername1.Text, txtbUsername2.Text, txtbPassword1.Text, txtbPassword2.Text, txtbGroup1.Text, txtbGroup2.Text);
                        this.Close();
                    }
                }
            }
        }

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
            dynamicTextboxesData.Clear();
            foreach (System.Windows.Forms.TextBox textBox in _allTextBoxesGroup1)
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    dynamicTextboxesData.Append(textBox.Text);
                    dynamicTextboxesData.Append("|"); ; // Use a separator between values
                }
            }
            Properties.Settings.Default.Groups1 = dynamicTextboxesData.ToString();

            dynamicTextboxesData.Clear(); // Clear for next set of textboxes
            foreach (System.Windows.Forms.TextBox textBox in _allTextBoxesGroup2)
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    dynamicTextboxesData.Append(textBox.Text);
                    dynamicTextboxesData.Append("|"); // Use a separator between values
                }
            }
            Properties.Settings.Default.Groups2 = dynamicTextboxesData.ToString();

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

            foreach (System.Windows.Forms.TextBox dynamicTextBox in _allTextBoxesGroup1)
            {
                if (_allTextBoxesGroup1.Count() != 1) Controls.Remove(dynamicTextBox);
            }
            _allTextBoxesGroup1.Clear();
            _allTextBoxesGroup1.Add(txtbGroups1);

            foreach (System.Windows.Forms.TextBox dynamicTextBox in _allTextBoxesGroup2)
            {
                if (_allTextBoxesGroup2.Count() != 1) Controls.Remove(dynamicTextBox);
            }
            _allTextBoxesGroup2.Clear();
            _allTextBoxesGroup2.Add(txtbGroups2);

            AdjustFormLayout();
        }

        #region Add & toggle OU textbox
        private void button1_Click(object sender, EventArgs e)
        {
            if (_allTextBoxesGroup1.Count < 5)
            {
                System.Windows.Forms.TextBox newTextBox = new System.Windows.Forms.TextBox();
                newTextBox.Size = txtbGroups1.Size; // Set the size
                if (_allTextBoxesGroup1.Count > 0)
                {
                    newTextBox.Location = new Point(txtbGroups1.Left, _allTextBoxesGroup1[_allTextBoxesGroup1.Count - 1].Bottom + 10);
                }
                else
                {
                    newTextBox.Location = new Point(txtbGroups1.Left, txtbGroups1.Bottom + 10);
                }
                _allTextBoxesGroup1.Add(newTextBox);
                Controls.Add(newTextBox);

                AdjustFormLayout();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (_allTextBoxesGroup1.Count > 1)
            {
                Controls.Remove(_allTextBoxesGroup1[_allTextBoxesGroup1.Count-1]);
                _allTextBoxesGroup1.RemoveAt(_allTextBoxesGroup1.Count-1);

                AdjustFormLayout();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_allTextBoxesGroup2.Count < 5)
            {
                System.Windows.Forms.TextBox newTextBox = new System.Windows.Forms.TextBox();
                newTextBox.Size = txtbGroups2.Size; // Set the size
                if (_allTextBoxesGroup2.Count > 0)
                {
                    newTextBox.Location = new Point(txtbGroups2.Left, _allTextBoxesGroup2[_allTextBoxesGroup2.Count - 1].Bottom + 10);
                }
                else
                {
                    newTextBox.Location = new Point(txtbGroups2.Left, txtbGroups2.Bottom + 10);
                }
                _allTextBoxesGroup2.Add(newTextBox);
                Controls.Add(newTextBox);

                AdjustFormLayout();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_allTextBoxesGroup2.Count > 1)
            {
                Controls.Remove(_allTextBoxesGroup2[_allTextBoxesGroup2.Count-1]);
                _allTextBoxesGroup2.RemoveAt(_allTextBoxesGroup2.Count-1);

                AdjustFormLayout();
            }
        }

        private void AdjustFormLayout()
        {
            int textboxNb = Math.Max(
                _allTextBoxesGroup1.Count,
                _allTextBoxesGroup2.Count
            );

            // Adjust the form's height to accommodate added textboxes
            this.Height =  _initialHeight + textboxNb * 30;
        }
        #endregion
    }
}
