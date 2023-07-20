using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADsFusion
{
    public partial class DisplayAccounts : Form
    {
        private Settings _settings;
        private ServerAndAdminLogin _login;
        private int _selectedListBoxIndex;

        private string _repositoryFilesPath;

        private string _server1;
        private string _server2;
        private string _serverLogin1;
        private string _serverLogin2;
        private string _serverPassword1;
        private string _serverPassword2;

        private string _userList1;
        private string _userList2;
        private string _mergedUserList;


        public DisplayAccounts()
        {
            InitializeComponent();

            _settings = new Settings();
            _login = new ServerAndAdminLogin();

            // Define the path to the repository
            _repositoryFilesPath = "C:\\ADsFusion\\Files";

            // Check and create the repository directory if it doesn't exist
            CheckAndCreateDirectory(_repositoryFilesPath);

            // Define the list of files with their paths and default content
            List<(string filePath, string defaultContent)> files = new List<(string filePath, string defaultContent)>
            {
                (Path.Combine(_repositoryFilesPath, "MergedUserList.csv"), ""),
                (Path.Combine(_repositoryFilesPath, "UserList1.csv"), ""),
                (Path.Combine(_repositoryFilesPath, "UserList2.csv"), ""),
                (Path.Combine(_repositoryFilesPath, "ServersSettings.csv"), ""),
                // Add more files here if needed
            };

            // Check and create each file if it doesn't exist
            foreach (var file in files)
            {
                CheckAndCreateFile(file.filePath, file.defaultContent);
            }
        }

        private void DisplayAccounts_Load(object sender, EventArgs e)
        {
            if (CheckIfLogged())
            {
                UpdateAll();
                DisplayUserList();
            }
            else
            {
                _login.ShowDialog();
            }
        }

        private bool CheckIfLogged()
        {
            return true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DisplayUserList();
        }

        private void DisplayUserList()
        {
            listBox1.Items.Clear();
            /*
            foreach (MergedUser user in _allUsers)
            {
                if (textBox1.Text == "$")
                {
                    if (user.AdresseElectronique21 == "-")
                    {
                        //ajout de l'utilisateur dans la textbox
                        listBox1.Items.Add(user.NomComplet1 + " / " + user.Identifiant2);
                    }
                }
                else if (user.Nom2.ToString().Normalize().Trim().ToLower().Contains(textBox1.Text.ToString().Normalize().Trim().ToLower()) || textBox1.Text.ToString().Normalize().Trim().ToLower().Contains(user.Nom2.ToString().Normalize().Trim().ToLower()) ||
                    user.Prenom2.ToString().Normalize().Trim().ToLower().Contains(textBox1.Text.ToString().Normalize().Trim().ToLower()) || textBox1.Text.ToString().Normalize().Trim().ToLower().Contains(user.Prenom2.ToString().Normalize().Trim().ToLower()) ||
                    user.Identifiant2.ToString().Normalize().Trim().ToLower().Contains(textBox1.Text.ToString().Normalize().Trim().ToLower()) || textBox1.Text.ToString().Normalize().Trim().ToLower().Contains(user.Identifiant2.ToString().Normalize().Trim().ToLower()))
                {
                    //ajout de l'utilisateur dans la textbox
                    listBox1.Items.Add(user.NomComplet1 + " / " + user.Identifiant2);
                }
            }
             */
        }

        private void CheckAndCreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                // Create the directory
                DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);
            }
        }

        private void CheckAndCreateFile(string filePath, string defaultContent)
        {
            if (!File.Exists(filePath))
            {
                // Create the file with default content
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    writer.Write(defaultContent);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _login.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _settings.ShowDialog();
        }

        #region Account list filter
        private void button1_Click(object sender, EventArgs e)
        {
            // Show the context menu strip at the button's location
            contextMenuStrip2.Show(button1, new Point(0, button1.Height));
        }

        private void aZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aZToolStripMenuItem.Checked = true;

            foreach (ToolStripMenuItem item in contextMenuStrip2.Items)
            {
                if (item != aZToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
        }

        private void zAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zAToolStripMenuItem.Checked = true;

            foreach (ToolStripMenuItem item in contextMenuStrip2.Items)
            {
                if (item != zAToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
        }
        #endregion

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int index = listBox1.IndexFromPoint(e.Location);
                if (index >= 0 && index < listBox1.Items.Count)
                {
                    listBox1.SelectedIndex = index;
                    _selectedListBoxIndex = index;
                }
            }
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listBox1.IndexFromPoint(e.Location);
                if (index >= 0 && index < listBox1.Items.Count && index == _selectedListBoxIndex)
                {
                    contextMenuStrip1.Show(listBox1, e.Location);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            UpdateUserList1();
            UpdateUserList2();
            MergeUserList();
            label1.Text = "Last : " + File.GetLastWriteTime(Path.Combine(_repositoryFilesPath, "MergedUserList.csv")).Date.ToString("dd.MM.yyyy");
        }

        private void UpdateUserList1()
        {

        }

        private void UpdateUserList2()
        {

        }

        private void MergeUserList()
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string readmeFilePath = @"C:\ADsFusion\README.txt";

            // Check if the file exists before trying to open it
            if (System.IO.File.Exists(readmeFilePath))
            {
                // Start the default text editor process and open the README.txt file
                Process.Start(readmeFilePath);
            }
            else
            {
                MessageBox.Show("The README.txt file does not exist.");
            }
        }
    }
}
