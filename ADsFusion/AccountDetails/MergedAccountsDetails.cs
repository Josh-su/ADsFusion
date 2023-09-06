using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ADsFusion
{
    public partial class AccountDetails : Form
    {
        public AccountDetails()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void AccountDetails_Load(object sender, EventArgs e)
        {
            if (Properties.CustomNames.Default.sAMAccountName1.ToString() != null) label1.Text = Properties.CustomNames.Default.sAMAccountName1;
            if (Properties.CustomNames.Default.sAMAccountName2.ToString() != null) label18.Text = Properties.CustomNames.Default.sAMAccountName2;
            if (Properties.CustomNames.Default.displayName1.ToString() != null) label2.Text = Properties.CustomNames.Default.displayName1;
            if (Properties.CustomNames.Default.displayName2.ToString() != null) label17.Text = Properties.CustomNames.Default.displayName2;
            if (Properties.CustomNames.Default.givenName1.ToString() != null) label3.Text = Properties.CustomNames.Default.givenName1;
            if (Properties.CustomNames.Default.givenName2.ToString() != null) label16.Text = Properties.CustomNames.Default.givenName2;
            if (Properties.CustomNames.Default.sn1.ToString() != null) label4.Text = Properties.CustomNames.Default.sn1;
            if (Properties.CustomNames.Default.sn2.ToString() != null) label15.Text = Properties.CustomNames.Default.sn2;
            if (Properties.CustomNames.Default.mail1.ToString() != null) label5.Text = Properties.CustomNames.Default.mail1;
            if (Properties.CustomNames.Default.mail2.ToString() != null) label14.Text = Properties.CustomNames.Default.mail2;
            if (Properties.CustomNames.Default.title1.ToString() != null) label6.Text = Properties.CustomNames.Default.title1;
            if (Properties.CustomNames.Default.title2.ToString() != null) label13.Text = Properties.CustomNames.Default.title2;
            if (Properties.CustomNames.Default.description1.ToString() != null) label7.Text = Properties.CustomNames.Default.description1;
            if (Properties.CustomNames.Default.description2.ToString() != null) label12.Text = Properties.CustomNames.Default.description2;
        }

        internal void InitializeWithUser(User user)
        {
            // Populate the form's controls with the user's information
            textBox1.Text = user.SAMAccountName1;
            textBox14.Text = user.SAMAccountName2;
            textBox2.Text = user.DisplayName1;
            textBox13.Text = user.DisplayName2;
            textBox3.Text = user.GivenName1;
            textBox12.Text = user.GivenName2;
            textBox4.Text = user.Sn1;
            textBox11.Text = user.Sn2;
            textBox5.Text = user.Mail1;
            textBox10.Text = user.Mail2;
            textBox6.Text = user.Title1;
            textBox9.Text = user.Title2;
            textBox7.Text = user.Description1;
            textBox8.Text = user.Description2;
            if (user.UserGroups1 != null) foreach (string group in user.UserGroups1) listBox1.Items.Add(group);
            if (user.UserGroups2 != null) foreach (string group in user.UserGroups2) listBox2.Items.Add(group);
        }
    }
}
