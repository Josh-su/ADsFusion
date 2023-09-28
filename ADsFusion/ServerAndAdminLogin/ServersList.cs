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
    public partial class ServersList : Form
    {
        private readonly Dictionary<string, ServerCredentials> _serverCredentialsForms;
        
        private MergeSettings _mergeSettings;

        /// <summary>
        /// 
        /// </summary>
        public ServersList()
        {
            InitializeComponent();

            // Set the initial state of the Button button4 to disabled
            button4.Enabled = false;

            _serverCredentialsForms = new Dictionary<string, ServerCredentials>();

            _mergeSettings = new MergeSettings();
        }

        private void ServersList_Load(object sender, EventArgs e)
        {
            LoadList();
        }

        private void LoadList()
        {
            listBox1.Items.Clear();
            // Assuming you have a maximum number of credentials, e.g., 5
            int maxCredentials = 5;

            for (int i = 1; i <= maxCredentials; i++)
            {
                string domain = Properties.Credentials.Default[$"Domain{i}"].ToString();
                string username = Properties.Credentials.Default[$"Username{i}"].ToString();

                if (!string.IsNullOrEmpty(domain) && !string.IsNullOrEmpty(username))
                {
                    listBox1.Items.Add($"{domain}, {username}");
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenServerCrendentialForm();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            // Assuming you have a maximum number of credentials, e.g., 5
            int maxCredentials = 5;

            if (listBox1.SelectedItem != null)
            {
                string selectedItemText = listBox1.SelectedItem.ToString();
                // Extract domain and username from the selected item text
                string[] parts = selectedItemText.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                string domain = parts[0];
                string username = parts[1];

                // Remove the selected item from the list
                listBox1.Items.Remove(listBox1.SelectedItem);

                // Find the corresponding setting and clear it
                for (int i = 1; i <= maxCredentials; i++)
                {
                    if (Properties.Credentials.Default[$"Domain{i}"].ToString() == domain && Properties.Credentials.Default[$"Username{i}"].ToString() == username)
                    {
                        // Clear other related settings as needed
                        Properties.Credentials.Default[$"Domain{i}"] = string.Empty;
                        Properties.Credentials.Default[$"Username{i}"] = string.Empty;
                        Properties.Credentials.Default[$"Password{i}"] = string.Empty;
                        Properties.Credentials.Default[$"GroupAdmin{i}"] = string.Empty;
                        Properties.Credentials.Default[$"Groups{i}"] = string.Empty;
                        
                        Properties.Credentials.Default.Save(); // Save changes
                        break;
                    }
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            OpenServerCrendentialForm(true);
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenServerCrendentialForm(true);
        }

        private void OpenServerCrendentialForm(bool modifying = false)
        {
            string selectedItemText = null;
            if (listBox1.SelectedItems.Count > 0)
            {
                selectedItemText = listBox1.SelectedItem.ToString();
            }

            switch (modifying)
            {
                case true:
                    if(!string.IsNullOrEmpty(selectedItemText)) 
                    {
                        if (!_serverCredentialsForms.ContainsKey(selectedItemText))
                        {
                            ServerCredentials newForm = new ServerCredentials();
                            _serverCredentialsForms.Add(selectedItemText, newForm);
                            newForm.FormClosed += (s, args) => _serverCredentialsForms.Remove(selectedItemText);
                            newForm.FormClosed += (s, args) => LoadList();
                            if (modifying)
                            {
                                newForm.Modifying = true;
                                newForm.InitializeCredential(selectedItemText);
                            }
                        }
                        // Show the form, whether it's a new instance or an existing one.
                        if (_serverCredentialsForms.ContainsKey(selectedItemText))
                        {
                            _serverCredentialsForms[selectedItemText].Show();
                            _serverCredentialsForms[selectedItemText].BringToFront();
                        }
                    }
                    break;
                case false:
                    ServerCredentials newForm1 = new ServerCredentials();
                    newForm1.Show();
                    newForm1.BringToFront();
                    newForm1.FormClosed += (s, args) => LoadList();
                    break;
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            _mergeSettings.ShowDialog();
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check the number of items in the ListBox
            if (listBox1.Items.Count >= 2)
            {
                // If there are 2 or more items, enable the button
                button4.Enabled = true;
            }
            else
            {
                // If there are less than 2 items, disable the button
                button4.Enabled = false;
            }
        }
    }
}