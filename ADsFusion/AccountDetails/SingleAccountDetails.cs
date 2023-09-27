using ADsFusion.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace ADsFusion
{
    public partial class SingleAccountDetails : Form
    {
        private User _user; // Declare a class-level variable to store the User object

        private List<List<string>> _userData;
        private List<List<string>> _CustomNamesData;

        /// <summary>
        /// 
        /// </summary>
        public SingleAccountDetails()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Set the MaximumSize and MinimumSize to the initial size of your form
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            // Enable KeyPreview
            this.KeyPreview = true;
        }

        private void SingleAccountDetails_Load(object sender, EventArgs e)
        {
            _userData = new List<List<string>>
            {
                new List<string> { _user.SAMAccountName1, _user.DisplayName1, _user.GivenName1, _user.Sn1, _user.Mail1, _user.Title1, _user.Description1 },
                new List<string> { _user.SAMAccountName2, _user.DisplayName2, _user.GivenName2, _user.Sn2, _user.Mail2, _user.Title2, _user.Description2 },
                new List<string> { _user.SAMAccountName3, _user.DisplayName3, _user.GivenName3, _user.Sn3, _user.Mail3, _user.Title3, _user.Description3 },
                new List<string> { _user.SAMAccountName4, _user.DisplayName4, _user.GivenName4, _user.Sn4, _user.Mail4, _user.Title4, _user.Description4 },
                new List<string> { _user.SAMAccountName5, _user.DisplayName5, _user.GivenName5, _user.Sn5, _user.Mail5, _user.Title5, _user.Description5 }
            };

            _CustomNamesData = new List<List<string>> { };
            for (int i = 1; i <= 5; i++)
            {
                List<string> data = new List<string>
                {
                    Properties.CustomNames.Default[$"sAMAccountName{i}"].ToString(),
                    Properties.CustomNames.Default[$"displayName{i}"].ToString(),
                    Properties.CustomNames.Default[$"givenName{i}"].ToString(),
                    Properties.CustomNames.Default[$"sn{i}"].ToString(),
                    Properties.CustomNames.Default[$"mail{i}"].ToString(),
                    Properties.CustomNames.Default[$"title{i}"].ToString(),
                    Properties.CustomNames.Default[$"description{i}"].ToString()
                };
                _CustomNamesData.Add(data);
            }

            // Call the method with the appropriate index based on the selected item
            int domainIndex = GetDomainIndex(_user.Domain);
            if (domainIndex >= 0)
            {
                LoadTextBoxes(domainIndex);
                LoadLabels(domainIndex);
                LoadListBox(domainIndex);
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

        private void LoadTextBoxes(int index)
        {
            List<string> userData = _userData[index];

            TextBox[] textBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7 };

            for (int i = 0; i < userData.Count; i++)
            {
                textBoxes[i].Text = userData[i] ?? "";
            }
        }

        private void LoadListBox(int index)
        {
            switch (index)
            {
                case 0:
                    foreach (string group in _user.UserGroups1) listBox1.Items.Add(group);
                    break;
                case 1:
                    foreach (string group in _user.UserGroups2) listBox1.Items.Add(group);
                    break;
                case 2:
                    foreach (string group in _user.UserGroups3) listBox1.Items.Add(group);
                    break;
                case 3:
                    foreach (string group in _user.UserGroups4) listBox1.Items.Add(group);
                    break;
                case 4:
                    foreach (string group in _user.UserGroups5) listBox1.Items.Add(group);
                    break;
            }
        }

        private void LoadLabels(int index)
        {
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
            this._user = user; // Store the user object in the class-level variable
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
