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
            UpdateGroups();
        }

        /// <summary>
        /// 
        /// </summary>
        public event ItemCheckEventHandler CheckedItemChanged
        {
            add { checkedListBox1.ItemCheck += value; }
            remove { checkedListBox1.ItemCheck -= value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler RadioButtonsCheckedChanged
        {
            add 
            {
                radioButton1.CheckedChanged += value;
                radioButton2.CheckedChanged += value;
            }
            remove
            {
                radioButton1.CheckedChanged -= value;
                radioButton2.CheckedChanged -= value;
            }
        }

        private void FilterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel the close operation to prevent the form from closing
            e.Cancel = true;
            // Hide the form instead
            this.Hide();
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
                if (filterText == "$" && _checkedItems.Contains(group))
                {
                    checkedListBox1.Items.Add(group, true);
                }
                else if (group.ToLower().Contains(filterText))
                {
                    checkedListBox1.Items.Add(group, _checkedItems.Contains(group));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateGroups()
        {
            // Clear existing items in the CheckedListBox 
            checkedListBox1.Items.Clear();
            // Add items from the groups list to the CheckedListBox
            foreach (string group in ListGroups)
            {
                checkedListBox1.Items.Add(group);
            }
        }

        private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string itemName = checkedListBox1.Items[e.Index].ToString();

            // If the item is being checked, add it to the _checkedItems list
            if (e.NewValue == CheckState.Checked)
            {
                if (!_checkedItems.Contains(itemName))
                {
                    _checkedItems.Add(itemName);
                }
            }
            // If the item is being unchecked, remove it from the _checkedItems list
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (_checkedItems.Contains(itemName))
                {
                    _checkedItems.Remove(itemName);
                }
            }
            SelectedGroups = _checkedItems; // Update the SelectedGroups property with the new list
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) SelectAllMatchingGroups = true;
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) SelectAllMatchingGroups = false;
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
