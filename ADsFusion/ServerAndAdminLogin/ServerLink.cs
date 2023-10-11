using ADsFusion.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADsFusion
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ServerLink : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Modifying = false;

        private int _index;
        private readonly string[] _servers = {
            Credentials.Default.Domain1,
            Credentials.Default.Domain2,
            Credentials.Default.Domain3,
            Credentials.Default.Domain4,
            Credentials.Default.Domain5
        };
        private readonly string[] _attributs = {
            "sAMAccountName",
            "displayName",
            "givenName",
            "sn",
            "mail",
            "title",
            "description"
        };

        // Filter out the empty strings using LINQ
        private readonly string[] nonEmptyServers;

        /// <summary>
        /// 
        /// </summary>
        public ServerLink()
        {
            InitializeComponent();

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

            nonEmptyServers = _servers.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            comboBox1.Items.AddRange(nonEmptyServers);
            comboBox2.Items.AddRange(nonEmptyServers);
            comboBox3.Items.AddRange(_attributs);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedServer = comboBox1.SelectedItem?.ToString();

            // If a server is selected in comboBox1, disable it in comboBox2
            if (!string.IsNullOrEmpty(selectedServer))
            {
                comboBox2.SelectedIndexChanged -= comboBox2_SelectedIndexChanged; // Unsubscribe temporarily
                string selecteditem = comboBox2.SelectedItem?.ToString();
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(nonEmptyServers);
                comboBox2.Items.Remove(selectedServer);
                comboBox2.SelectedItem = selecteditem;
                comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged; // Re-subscribe
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedServer = comboBox2.SelectedItem?.ToString();

            // If a server is selected in comboBox2, disable it in comboBox1
            if (!string.IsNullOrEmpty(selectedServer))
            {
                string selecteditem = comboBox1.SelectedItem?.ToString();

                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged; // Unsubscribe temporarily
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(nonEmptyServers);
                comboBox1.Items.Remove(selectedServer);
                comboBox1.SelectedItem = selecteditem;
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged; // Re-subscribe
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemText"></param>
        public void InitializeLink(string itemText)
        {
            // Call the method with the appropriate index based on the selected item
            // Assuming you have a maximum number of credentials
            int maxCredentials = 10;

            for (int i = 1; i <= maxCredentials; i++)
            {
                if (itemText == Links.Default[$"Link{i}"].ToString())
                {
                    LoadLink(i);
                    break; // Exit the loop after Load
                }
            }
        }

        private void LoadLink(int index)
        {
            _index = index;

            // Split the input string using the comma and colon as separators
            string[] splitStrings = Links.Default[$"Link{_index}"].ToString().Split(new char[] { ',', ':' });

            // Trim each element to remove leading or trailing spaces
            string server1 = splitStrings[0].Trim();
            string server2 = splitStrings[1].Trim();
            string attribut = splitStrings[2].Trim();

            comboBox1.Text = server1;
            comboBox2.Text = server2;
            comboBox3.Text = attribut;
        }

        private void SaveLink(string link)
        {
            int maxSettings = 10; // Define the maximum number of Links

            for (int i = 1; i <= maxSettings; i++)
            {
                if (string.IsNullOrEmpty((string)Links.Default[$"Link{i}"]))
                {
                    Links.Default[$"Link{i}"] = link;
                    break; // Exit the loop after saving the first empty slot
                }
            }
        }

        private void SaveModifiedLink(string link)
        {
            Links.Default[$"Link{_index}"] = link;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(comboBox3.Text)) MessageBox.Show("Veuillez choisir toutes les combobox");
            else
            {
                string link = $"{comboBox1.Text}, {comboBox2.Text}: {comboBox3.Text}";

                if (Modifying) SaveModifiedLink(link);
                else SaveLink(link);

                Links.Default.Save(); // Save changes to settings
                this.Close();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
