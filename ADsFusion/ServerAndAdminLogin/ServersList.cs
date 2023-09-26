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
        private ServerCredentials _serverCredentials;

        /// <summary>
        /// 
        /// </summary>
        public ServersList()
        {
            InitializeComponent();

            _serverCredentials = new ServerCredentials();
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
            _serverCredentials.ShowDialog();
            LoadList();
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
            if(listBox1.SelectedItems.Count > 0)
            {
                _serverCredentials.Modifying = true;
                _serverCredentials.InitializeCredential(listBox1.SelectedItem.ToString());
                _serverCredentials.ShowDialog();
                LoadList();
            }
        }
    }
}