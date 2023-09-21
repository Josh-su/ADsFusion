using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace ADsFusion
{
    public partial class SingleAccountDetails : Form
    {
        private User _user; // Declare a class-level variable to store the User object

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
            // Check if the user object is not null and set labels accordingly
            if (_user != null)
            {
                label1.Text = _user.SAMAccountName1 != null
                    ? Properties.CustomNames.Default.sAMAccountName1
                    : Properties.CustomNames.Default.sAMAccountName2;

                label2.Text = _user.SAMAccountName1 != null
                    ? Properties.CustomNames.Default.displayName1
                    : Properties.CustomNames.Default.displayName2;

                label3.Text = _user.SAMAccountName1 != null
                    ? Properties.CustomNames.Default.givenName1
                    : Properties.CustomNames.Default.givenName2;

                label4.Text = _user.SAMAccountName1 != null
                    ? Properties.CustomNames.Default.sn1
                    : Properties.CustomNames.Default.sn2;

                label5.Text = _user.SAMAccountName1 != null
                    ? Properties.CustomNames.Default.mail1
                    : Properties.CustomNames.Default.mail2;

                label6.Text = _user.SAMAccountName1 != null
                    ? Properties.CustomNames.Default.title1
                    : Properties.CustomNames.Default.title2;

                label7.Text = _user.SAMAccountName1 != null
                    ? Properties.CustomNames.Default.description1
                    : Properties.CustomNames.Default.description2;
            }
        }

        internal void InitializeWithUser(User user)
        {
            this._user = user; // Store the user object in the class-level variable

            // Populate the form's controls with the user's information
            string[] userData = _user.SAMAccountName1 != null
                ? new string[] { _user.SAMAccountName1, _user.DisplayName1, _user.GivenName1, _user.Sn1, _user.Mail1, _user.Title1, _user.Description1 }
                : new string[] { _user.SAMAccountName2, _user.DisplayName2, _user.GivenName2, _user.Sn2, _user.Mail2, _user.Title2, _user.Description2 };

            System.Windows.Forms.TextBox[] textBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7 };

            for (int i = 0; i < userData.Length; i++)
            {
                textBoxes[i].Text = userData[i] ?? "";
            }

            foreach (string group in _user.SAMAccountName1 != null ? _user.UserGroups1 : _user.UserGroups2)
            {
                listBox1.Items.Add(group);
            }
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
