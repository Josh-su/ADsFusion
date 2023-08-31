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
        public List<string> ListGroups;
        public List<string> SelectedGroups;

        private List<string> _checkedItems;

        public FilterForm()
        {
            InitializeComponent();

            // Set the StartPosition to Manual
            this.StartPosition = FormStartPosition.Manual;

            // Attach the FormClosing event handler
            this.FormClosing += FilterForm_FormClosing;

            ListGroups = new List<string>();
            _checkedItems = new List<string>();
            SelectedGroups = new List<string>();
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            UpdateGroups(ListGroups);
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
            _checkedItems.Clear(); // Clear the existing checked items

            foreach (var item in checkedListBox1.CheckedItems)
            {
                _checkedItems.Add(item.ToString()); // Add each checked item to the list
            }

            SelectedGroups = _checkedItems; // Update the SelectedGroups property with the new list
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _checkedItems.Clear();

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Clear existing items in the CheckedListBox 
            checkedListBox1.Items.Clear();

            string filterText = textBox1.Text.ToLower();

            foreach (string group in ListGroups)
            {
                if (filterText == "check" && _checkedItems.Contains(group))
                {
                    checkedListBox1.Items.Add(group, true);
                }
                else if (_checkedItems.Contains(group) || group.ToLower().Contains(filterText))
                {
                    checkedListBox1.Items.Add(group, _checkedItems.Contains(group));
                }
            }
        }

        public void UpdateGroups(List<string> groups)
        {
            // Clear existing items in the CheckedListBox 
            checkedListBox1.Items.Clear();
            // Add items from the groups list to the CheckedListBox
            foreach (string group in groups)
            {
                checkedListBox1.Items.Add(group);
            }
        }
    }
}
