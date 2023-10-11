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
    public partial class MergeSettings : Form
    {
        private readonly Dictionary<string, ServerLink> _serverLinkForms;

        /// <summary>
        /// 
        /// </summary>
        public MergeSettings()
        {
            InitializeComponent();

            _serverLinkForms = new Dictionary<string, ServerLink>();
        }

        private void MergeSettings_Load(object sender, EventArgs e)
        {
            LoadList();
        }

        private void LoadList()
        {
            listBox1.Items.Clear();
            // Assuming you have a maximum number of credentials
            int maxCredentials = 10;

            for (int i = 1; i <= maxCredentials; i++)
            {
                string link = Properties.Links.Default[$"Link{i}"].ToString();

                if (!string.IsNullOrEmpty(link))
                {
                    listBox1.Items.Add(link);
                }
            }
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenServerLinkForm(true);
        }

        private void OpenServerLinkForm(bool modifying = false)
        {
            string selectedItemText = null;
            if (listBox1.SelectedItems.Count > 0)
            {
                selectedItemText = listBox1.SelectedItem.ToString();
            }

            switch (modifying)
            {
                case true:
                    if (!string.IsNullOrEmpty(selectedItemText))
                    {
                        if (!_serverLinkForms.ContainsKey(selectedItemText))
                        {
                            ServerLink newForm = new ServerLink();
                            _serverLinkForms.Add(selectedItemText, newForm);
                            newForm.FormClosed += (s, args) => _serverLinkForms.Remove(selectedItemText);
                            newForm.FormClosed += (s, args) => LoadList();
                            if (modifying)
                            {
                                newForm.Modifying = true;
                                newForm.InitializeLink(selectedItemText);
                            }
                        }
                        // Show the form, whether it's a new instance or an existing one.
                        if (_serverLinkForms.ContainsKey(selectedItemText))
                        {
                            _serverLinkForms[selectedItemText].Show();
                            _serverLinkForms[selectedItemText].BringToFront();
                        }
                    }
                    break;
                case false:
                    ServerLink newForm1 = new ServerLink();
                    newForm1.Show();
                    newForm1.BringToFront();
                    newForm1.FormClosed += (s, args) => LoadList();
                    break;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenServerLinkForm();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            OpenServerLinkForm(true);
        }

        private void Button3_Click(object sender, EventArgs e)
        {            
            // Assuming you have a maximum number of credentials
            int maxCredentials = 10;

            if (listBox1.SelectedItem != null)
            {
                string selectedItemText = listBox1.SelectedItem.ToString();

                // Remove the selected item from the list
                listBox1.Items.Remove(listBox1.SelectedItem);

                // Find the corresponding setting and clear it
                for (int i = 1; i <= maxCredentials; i++)
                {
                    if (Properties.Links.Default[$"Link{i}"].ToString() == selectedItemText)
                    {
                        // Clear other related settings as needed
                        Properties.Links.Default[$"Link{i}"] = string.Empty;

                        Properties.Links.Default.Save(); // Save changes
                        break;
                    }
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
