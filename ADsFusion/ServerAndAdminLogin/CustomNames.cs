using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADsFusion
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CustomNames : Form
    {
        private int _index;

        /// <summary>
        /// 
        /// </summary>
        public CustomNames()
        {
            InitializeComponent();
        }

        private void CustomNames_Load(object sender, EventArgs e)
        {
            switch (_index)
            {
                case 1:
                    LoadCustomNames(1);
                    break;
                case 2:
                    LoadCustomNames(2);
                    break;
                case 3:
                    LoadCustomNames(3);
                    break;
                case 4:
                    LoadCustomNames(4);
                    break;
                case 5:
                    LoadCustomNames(5);
                    break;
            }
        }

        private void LoadCustomNames(int index)
        {
            sAMAccountName1.Text = Properties.CustomNames.Default[$"sAMAccountName{index}"].ToString();
            displayName1.Text = Properties.CustomNames.Default[$"displayName{index}"].ToString();
            givenName1.Text = Properties.CustomNames.Default[$"givenName{index}"].ToString();
            sn1.Text = Properties.CustomNames.Default[$"sn{index}"].ToString();
            mail1.Text = Properties.CustomNames.Default[$"mail{index}"].ToString();
            title1.Text = Properties.CustomNames.Default[$"title{index}"].ToString();
            description1.Text = Properties.CustomNames.Default[$"description{index}"].ToString();
        }

        private void SetDefault()
        {
            sAMAccountName1.Text = "sAMAccountName";
            displayName1.Text = "displayName";
            givenName1.Text = "givenName";
            sn1.Text = "sn";
            mail1.Text = "mail";
            title1.Text = "title";
            description1.Text = "description";
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

        private void SaveProperties()
        {
            Properties.CustomNames.Default[$"sAMAccountName{_index}"] = sAMAccountName1.Text;
            Properties.CustomNames.Default[$"displayName{_index}"] = displayName1.Text;
            Properties.CustomNames.Default[$"givenName{_index}"] = givenName1.Text;
            Properties.CustomNames.Default[$"sn{_index}"] = sn1.Text;
            Properties.CustomNames.Default[$"mail{_index}"] = mail1.Text;
            Properties.CustomNames.Default[$"title{_index}"] = title1.Text;
            Properties.CustomNames.Default[$"description{_index}"] = description1.Text;

            // Save the changes
            Properties.CustomNames.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void Initialize(int index)
        {
            _index = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetDefault();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetDefaultIfEmpty();
            SaveProperties();
            this.Close();
        }
    }
}
