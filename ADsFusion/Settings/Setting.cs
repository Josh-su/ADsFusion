using System;
using ADsFusion.Properties;
using System.Windows.Forms;

namespace ADsFusion
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Setting : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public Setting()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Links.Default.Reset();
            CustomNames.Default.Reset();
            Credentials.Default.Reset();
            Settings.Default.Reset();
        }
    }
}
