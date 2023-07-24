using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
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
            if (CheckIfLogged() == 0)
            {
                _login.ShowDialog();
            }
            else 
            {
                UpdateAll(CheckIfLogged());
                DisplayUserList();
            }
        }

        private int CheckIfLogged()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Domain1) && string.IsNullOrEmpty(Properties.Settings.Default.Domain2))
            {
                return 0;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Domain1) && string.IsNullOrEmpty(Properties.Settings.Default.Domain2))
            {
                return 1;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Domain2) && string.IsNullOrEmpty(Properties.Settings.Default.Domain1))
            {
                return 2;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Domain1) && !string.IsNullOrEmpty(Properties.Settings.Default.Domain2))
            {
                return 3;
            }
            return 0;
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
            UpdateAll(CheckIfLogged());
        }

        private void UpdateAll(int x)
        {
            switch (x)
            {
                case 0:
                    _login.ShowDialog();
                    break;
                case 1:
                    UpdateUserList1();
                    DateTime lastWriteTime1 = File.GetLastWriteTime(Path.Combine(_repositoryFilesPath, "UserList1.csv"));
                    DateTime localTime1 = lastWriteTime1.ToLocalTime();
                    label1.Text = "Last update: " + localTime1.ToString("dd.MM.yyyy HH:mm:ss");
                    break;
                case 2:
                    UpdateUserList2();
                    DateTime lastWriteTime2 = File.GetLastWriteTime(Path.Combine(_repositoryFilesPath, "UserList2.csv"));
                    DateTime localTime2 = lastWriteTime2.ToLocalTime();
                    label1.Text = "Last update: " + localTime2.ToString("dd.MM.yyyy HH:mm:ss");
                    break;
                case 3:
                    UpdateUserList1();
                    UpdateUserList2();
                    MergeUserList();
                    DateTime lastWriteTime = File.GetLastWriteTime(Path.Combine(_repositoryFilesPath, "MergedUserList.csv"));
                    DateTime localTime = lastWriteTime.ToLocalTime();
                    label1.Text = "Last update: " + localTime.ToString("dd.MM.yyyy HH:mm:ss");
                    break;
            }
        }

        private void UpdateUserList1()
        {
            /*using (var context = new PrincipalContext(ContextType.Domain, "dgep.edu-vaud.ch", "OU=Groups,OU=GYREN,OU=EDU,DC=dgep,DC=edu-vaud,DC=ch"))
            {
                GroupPrincipal group = GroupPrincipal.FindByIdentity(context, IdentityType.Name, "UUS_GYREN");

                var allUsers = new List<UserPrincipal>();

                if (group != null)// Get all the users in the groups
                {
                    var members = group.GetMembers(true);

                    foreach (var member in members)
                    {
                        if (member is UserPrincipal user && user != null && user.Enabled.HasValue && user.Enabled.Value)
                        {
                            // Check if the user already exists in the list
                            if (allUsers.Any(u => u.SamAccountName == user.SamAccountName))
                            {
                                continue; // Skip adding the user
                            }
                            else
                            {
                                allUsers.Add(user);
                            }
                        }
                    }
                }

                // Clear the list before updating it
                _allEduvaudUsers.Clear();

                var progressCounter = 0;
                var totalUsers = allUsers.Count;

                // Convert the UserPrincipal objects to EduvaudUser objects and add them to 
                foreach (var user in allUsers)
                {
                    var groupsMembership = user.GetGroups();
                    var groups = new List<string>();
                    foreach (var groupMembership in groupsMembership)
                    {
                        groups.Add(groupMembership.Name);
                    }

                    // Get the underlying DirectoryEntry object
                    var de = user.GetUnderlyingObject() as DirectoryEntry;

                    EduvaudUser userToAdd = new(
                        Convert.ToString(user.GivenName),
                        Convert.ToString(user.Surname),
                        Convert.ToString(user.Name),
                        Convert.ToString(user.SamAccountName),
                        Convert.ToString(user.EmailAddress),
                        Convert.ToString(de.Properties["extensionAttribute2"].Value?.ToString()),
                        groups);

                    _allEduvaudUsers.Add(userToAdd);

                    progressCounter++;
                    backgroundWorker1.ReportProgress((int)(((double)progressCounter / totalUsers) * 100));
                }
            }
            WriteJson(_allEduvaudUsers, filePath);*/
        }

        private void UpdateUserList2()
        {

        }

        private void MergeUserList()
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string websiteUrl = "https://github.com/Josh-su/ADsFusion";

            try
            {
                Process.Start(websiteUrl);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur (e.g., if the default web browser is not found).
                MessageBox.Show("Error opening website: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
