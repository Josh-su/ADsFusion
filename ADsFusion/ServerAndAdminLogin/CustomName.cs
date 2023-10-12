using System;
using System.Windows.Forms;
using ADsFusion.Properties;

namespace ADsFusion
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CustomName : Form
    {
        private int _index;

        /// <summary>
        /// 
        /// </summary>
        public CustomName()
        {
            InitializeComponent();
        }

        private void CustomName_Load(object sender, EventArgs e)
        {
            switch (_index)
            {
                case 1:
                    LoadCustomName(1);
                    break;
                case 2:
                    LoadCustomName(2);
                    break;
                case 3:
                    LoadCustomName(3);
                    break;
                case 4:
                    LoadCustomName(4);
                    break;
                case 5:
                    LoadCustomName(5);
                    break;
            }
        }

        private void LoadCustomName(int index)
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
            CustomNames.Default[$"sAMAccountName{_index}"] = sAMAccountName1.Text;
            CustomNames.Default[$"displayName{_index}"] = displayName1.Text;
            CustomNames.Default[$"givenName{_index}"] = givenName1.Text;
            CustomNames.Default[$"sn{_index}"] = sn1.Text;
            CustomNames.Default[$"mail{_index}"] = mail1.Text;
            CustomNames.Default[$"title{_index}"] = title1.Text;
            CustomNames.Default[$"description{_index}"] = description1.Text;

            // Save the changes
            CustomNames.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void Initialize(int index)
        {
            _index = index;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SetDefault();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SetDefaultIfEmpty();
            SaveProperties();
            this.Close();
        }
    }
}
