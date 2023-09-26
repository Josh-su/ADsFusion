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
    /// <summary>
    /// 
    /// </summary>
    public partial class Settings : Form
    {
        private TextBox[] _textBoxes = new TextBox[35];
        private string[] _defaultValues = {"sAMAccountName", "displayName", "givenName", "sn", "mail", "title", "description"};

        /// <summary>
        /// 
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            List<int> ints = new List<int>();

            for (int i = 1; i <= 5; i++) // Assuming you have 5 settings
            {
                string domain = Properties.Credentials.Default[$"Domain{i}"].ToString();

                if (!string.IsNullOrEmpty(domain))
                {
                    ints.Add(i);
                }
            }
            SetParameters(ints);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SetDefaultIfEmpty();

            // Update the informations
            Properties.CustomNames.Default.sAMAccountName1 = sAMAccountName1.Text;
            Properties.CustomNames.Default.displayName1 = displayName1.Text;
            Properties.CustomNames.Default.givenName1 = givenName1.Text;
            Properties.CustomNames.Default.sn1 = sn1.Text;
            Properties.CustomNames.Default.mail1 = mail1.Text;
            Properties.CustomNames.Default.title1 = title1.Text;
            Properties.CustomNames.Default.description1 = description1.Text;

            Properties.CustomNames.Default.sAMAccountName2 = sAMAccountName2.Text;
            Properties.CustomNames.Default.displayName2 = displayName2.Text;
            Properties.CustomNames.Default.givenName2 = givenName2.Text;
            Properties.CustomNames.Default.sn2 = sn2.Text;
            Properties.CustomNames.Default.mail2 = mail2.Text;
            Properties.CustomNames.Default.title2 = title2.Text;
            Properties.CustomNames.Default.description2 = description2.Text;

            Properties.CustomNames.Default.sAMAccountName3 = sAMAccountName4.Text;
            Properties.CustomNames.Default.displayName3 = description3.Text;
            Properties.CustomNames.Default.givenName3 = title3.Text;
            Properties.CustomNames.Default.sn3 = mail3.Text;
            Properties.CustomNames.Default.mail3 = sn3.Text;
            Properties.CustomNames.Default.title3 = givenName3.Text;
            Properties.CustomNames.Default.description3 = displayName3.Text;

            // Save the changes
            Properties.CustomNames.Default.Save();

            this.Close();
        }


        private void LoadCustomNames(string domain)
        {
            for (int i = 1; i <= 5; i++)
            {
                string propertyName = $"sAMAccountName{i}";
                if (domain == Properties.CustomNames.Default[propertyName].ToString())
                {
                    sAMAccountName1.Text = Properties.CustomNames.Default[propertyName].ToString();
                    displayName1.Text = Properties.CustomNames.Default[$"displayName{i}"].ToString();
                    givenName1.Text = Properties.CustomNames.Default[$"givenName{i}"].ToString();
                    sn1.Text = Properties.CustomNames.Default[$"sn{i}"].ToString();
                    mail1.Text = Properties.CustomNames.Default[$"mail{i}"].ToString();
                    title1.Text = Properties.CustomNames.Default[$"title{i}"].ToString();
                    description1.Text = Properties.CustomNames.Default[$"description{i}"].ToString();
                    break;
                }
            }
        }

        private void SetDefaultIfEmpty()
        {
            if (string.IsNullOrEmpty(sAMAccountName1.Text)) sAMAccountName1.Text = "sAMAccountName";
            if (string.IsNullOrEmpty(displayName1.Text)) displayName1.Text = "displayName";
            if (string.IsNullOrEmpty(givenName1.Text)) givenName1.Text = "givenName";
            if (string.IsNullOrEmpty(sn1.Text)) sn1.Text = "sn";
            if (string.IsNullOrEmpty(mail1.Text)) mail1.Text = "mail";
            if (string.IsNullOrEmpty(title1.Text)) title1.Text = "title";
            if (string.IsNullOrEmpty(description1.Text)) description1.Text = "description";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Properties.CustomNames.Default.Reset();
        }
    }
}
