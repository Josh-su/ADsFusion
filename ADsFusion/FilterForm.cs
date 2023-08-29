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
    public partial class FilterForm : Form
    {
        public FilterForm()
        {
            InitializeComponent();

            // Set the StartPosition to Manual
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;

            // Attach the FormClosing event handler
            this.FormClosing += FilterForm_FormClosing;
        }

        private void FilterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel the close operation to prevent the form from closing
            e.Cancel = true;
            // Hide the form instead
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
