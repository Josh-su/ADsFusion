using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADsFusion
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            SetParameters();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetDefaultIfEmpty();

            // Update the informations
            Properties.CustomNames.Default.sAMAccountName1 = textBox1.Text;
            Properties.CustomNames.Default.sAMAccountName2 = textBox14.Text;
            Properties.CustomNames.Default.displayName1 = textBox2.Text;
            Properties.CustomNames.Default.displayName2 = textBox13.Text;
            Properties.CustomNames.Default.givenName1 = textBox3.Text;
            Properties.CustomNames.Default.givenName2 = textBox12.Text;
            Properties.CustomNames.Default.sn1 = textBox4.Text;
            Properties.CustomNames.Default.sn2 = textBox11.Text;
            Properties.CustomNames.Default.mail1 = textBox5.Text;
            Properties.CustomNames.Default.mail2 = textBox10.Text;
            Properties.CustomNames.Default.title1 = textBox6.Text;
            Properties.CustomNames.Default.title2 = textBox9.Text;
            Properties.CustomNames.Default.description1 = textBox7.Text;
            Properties.CustomNames.Default.description2 = textBox8.Text;

            foreach (RadioButton radioButton in this.Controls.OfType<RadioButton>())
            {
                if (radioButton.Checked)
                {
                    Properties.CustomNames.Default.MergeParameter = radioButton.Tag.ToString();
                }
            }

            // Save the changes
            Properties.CustomNames.Default.Save();
        }


        private void SetParameters()
        {
            if (Properties.CustomNames.Default.sAMAccountName1.ToString() != null) textBox1.Text = Properties.CustomNames.Default.sAMAccountName1;
            if (Properties.CustomNames.Default.sAMAccountName2.ToString() != null) textBox14.Text = Properties.CustomNames.Default.sAMAccountName2;
            if (Properties.CustomNames.Default.displayName1.ToString() != null) textBox2.Text = Properties.CustomNames.Default.displayName1;
            if (Properties.CustomNames.Default.displayName2.ToString() != null) textBox13.Text = Properties.CustomNames.Default.displayName2;
            if (Properties.CustomNames.Default.givenName1.ToString() != null) textBox3.Text = Properties.CustomNames.Default.givenName1;
            if (Properties.CustomNames.Default.givenName2.ToString() != null) textBox12.Text = Properties.CustomNames.Default.givenName2;
            if (Properties.CustomNames.Default.sn1.ToString() != null) textBox4.Text = Properties.CustomNames.Default.sn1;
            if (Properties.CustomNames.Default.sn2.ToString() != null) textBox11.Text = Properties.CustomNames.Default.sn2;
            if (Properties.CustomNames.Default.mail1.ToString() != null) textBox5.Text = Properties.CustomNames.Default.mail1;
            if (Properties.CustomNames.Default.mail2.ToString() != null) textBox10.Text = Properties.CustomNames.Default.mail2;
            if (Properties.CustomNames.Default.title1.ToString() != null) textBox6.Text = Properties.CustomNames.Default.title1;
            if (Properties.CustomNames.Default.title2.ToString() != null) textBox9.Text = Properties.CustomNames.Default.title2;
            if (Properties.CustomNames.Default.description1.ToString() != null) textBox7.Text = Properties.CustomNames.Default.description1;
            if (Properties.CustomNames.Default.description2.ToString() != null) textBox8.Text = Properties.CustomNames.Default.description2;

            if (string.IsNullOrEmpty(textBox1.Text)) textBox1.Text = "sAMAccountName";
            if (string.IsNullOrEmpty(textBox2.Text)) textBox2.Text = "displayName";
            if (string.IsNullOrEmpty(textBox3.Text)) textBox3.Text = "givenName";
            if (string.IsNullOrEmpty(textBox4.Text)) textBox4.Text = "sn";
            if (string.IsNullOrEmpty(textBox5.Text)) textBox5.Text = "mail";
            if (string.IsNullOrEmpty(textBox6.Text)) textBox6.Text = "title";
            if (string.IsNullOrEmpty(textBox7.Text)) textBox7.Text = "description";

            if (string.IsNullOrEmpty(textBox14.Text)) textBox14.Text = "sAMAccountName";
            if (string.IsNullOrEmpty(textBox13.Text)) textBox13.Text = "displayName";
            if (string.IsNullOrEmpty(textBox12.Text)) textBox12.Text = "givenName";
            if (string.IsNullOrEmpty(textBox11.Text)) textBox11.Text = "sn";
            if (string.IsNullOrEmpty(textBox10.Text)) textBox10.Text = "mail";
            if (string.IsNullOrEmpty(textBox9.Text)) textBox9.Text = "title";
            if (string.IsNullOrEmpty(textBox8.Text)) textBox8.Text = "description";

            foreach (RadioButton radioButton in this.Controls.OfType<RadioButton>())
            {
                // The Tag property of each radio button should be set to the corresponding parameter value
                if (radioButton.Tag != null && radioButton.Tag.ToString() == Properties.CustomNames.Default.MergeParameter)
                {
                    radioButton.Checked = true;
                }
                else
                {
                    radioButton.Checked = false;
                }
            }
        }

        private void SetDefaultIfEmpty()
        {
            if (string.IsNullOrEmpty(textBox1.Text)) textBox1.Text = "sAMAccountName";
            if (string.IsNullOrEmpty(textBox2.Text)) textBox2.Text = "displayName";
            if (string.IsNullOrEmpty(textBox3.Text)) textBox3.Text = "givenName";
            if (string.IsNullOrEmpty(textBox4.Text)) textBox4.Text = "sn";
            if (string.IsNullOrEmpty(textBox5.Text)) textBox5.Text = "mail";
            if (string.IsNullOrEmpty(textBox6.Text)) textBox6.Text = "title";
            if (string.IsNullOrEmpty(textBox7.Text)) textBox7.Text = "description";

            if (string.IsNullOrEmpty(textBox14.Text)) textBox14.Text = "sAMAccountName";
            if (string.IsNullOrEmpty(textBox13.Text)) textBox13.Text = "displayName";
            if (string.IsNullOrEmpty(textBox12.Text)) textBox12.Text = "givenName";
            if (string.IsNullOrEmpty(textBox11.Text)) textBox11.Text = "sn";
            if (string.IsNullOrEmpty(textBox10.Text)) textBox10.Text = "mail";
            if (string.IsNullOrEmpty(textBox9.Text)) textBox9.Text = "title";
            if (string.IsNullOrEmpty(textBox8.Text)) textBox8.Text = "description";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.CustomNames.Default.Reset();
            foreach (RadioButton radioButton in this.Controls.OfType<RadioButton>())
            {
                radioButton.Checked = false;
            }
        }
    }
}
