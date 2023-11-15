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
    public partial class FilterForm : Form
    {
        public List<string> ListGroups;
        public List<string> SelectedGroups;
        public bool SelectAllMatchingGroups;

        private readonly List<string> _checkedItems;

        /// <summary>
        /// 
        /// </summary>
        public FilterForm()
        {
            InitializeComponent();

            // Set the StartPosition to Manual
            this.StartPosition = FormStartPosition.Manual;

            // Attach the FormClosing event handler
            this.FormClosing += FilterForm_FormClosing;

            // Set the MaximumSize and MinimumSize to the initial size of your form
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            ListGroups = new List<string>();
            _checkedItems = new List<string>();
            SelectedGroups = new List<string>();

            // Enable KeyPreview
            this.KeyPreview = true;
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            
        }

        private void FilterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel the close operation to prevent the form from closing
            e.Cancel = true;
            // Hide the form instead
            this.Hide();
        }

        private void FilterForm_KeyDown(object sender, KeyEventArgs e)
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
