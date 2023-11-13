using ADsFusion.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ADsFusion
{
    public partial class SingleAccountDetails : Form
    {
        private User _user; // Declare a class-level variable to store the User object

        //private List<List<string>> _userData;
        private List<List<string>> _CustomNamesData;

        /// <summary>
        /// 
        /// </summary>
        public SingleAccountDetails()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;

            // Set the MaximumSize and MinimumSize to the initial size of your form
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            // Enable KeyPreview
            this.KeyPreview = true;
        }

        private void SingleAccountDetails_Load(object sender, EventArgs e)
        {
            _CustomNamesData = new List<List<string>> { };
            for (int i = 1; i <= 5; i++)
            {
                List<string> data = new List<string>
                {
                    CustomNames.Default[$"sAMAccountName{i}"].ToString(),
                    CustomNames.Default[$"displayName{i}"].ToString(),
                    CustomNames.Default[$"givenName{i}"].ToString(),
                    CustomNames.Default[$"sn{i}"].ToString(),
                    CustomNames.Default[$"mail{i}"].ToString(),
                    CustomNames.Default[$"title{i}"].ToString(),
                    CustomNames.Default[$"description{i}"].ToString()
                };
                _CustomNamesData.Add(data);
            }

            // Call the method with the appropriate index based on the selected item
            int domainIndex = GetDomainIndex(_user.Domain);
            if (domainIndex >= 0)
            {
                LoadLabels(domainIndex);
                LoadTextBoxes();
                LoadListBox();
            }
        }

        private int GetDomainIndex(string domain)
        {
            if (domain == Credentials.Default.Domain1)
            {
                return 0;
            }
            else if (domain == Credentials.Default.Domain2)
            {
                return 1;
            }
            else if (domain == Credentials.Default.Domain3)
            {
                return 2;
            }
            else if (domain == Credentials.Default.Domain4)
            {
                return 3;
            }
            else if (domain == Credentials.Default.Domain5)
            {
                return 4;
            }
            else
            {
                return -1;
            }
        }

        private void LoadTextBoxes()
        {
            List<string> userData = new List<string> { _user.SAMAccountName, _user.DisplayName, _user.GivenName, _user.Sn, _user.Mail, _user.Title, _user.Description };

            TextBox[] textBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7 };

            for (int i = 0; i < userData.Count; i++)
            {
                textBoxes[i].Text = userData[i] ?? "";
            }
        }

        private void LoadListBox()
        {
            foreach (string group in GetAD.GetADUserGroups(_user.Domain, _user.SAMAccountName)) listBox1.Items.Add(group);
        }

        private void LoadLabels(int index)
        {
            label9.Text = _user.Domain;
            label1.Text = _CustomNamesData[index][0];
            label2.Text = _CustomNamesData[index][1];
            label3.Text = _CustomNamesData[index][2];
            label4.Text = _CustomNamesData[index][3];
            label5.Text = _CustomNamesData[index][4];
            label6.Text = _CustomNamesData[index][5];
            label7.Text = _CustomNamesData[index][6];
        }

        internal void InitializeWithUser(User user)
        {
            _user = user; // Store the user object in the class-level variable
        }

        private void SingleAccountDetails_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is Escape (Esc)
            if (e.KeyCode == Keys.Escape)
            {
                // Close the form
                this.Close();
            }
        }
    }
}
