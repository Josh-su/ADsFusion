using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting.Contexts;

namespace ADsFusion
{
    public partial class DisplayAccounts : Form
    {
        private Settings _settings;
        private ServerAndAdminLogin _login;
        private int _selectedListBoxIndex;

        private string _repositoryFilesPath;

        private string _domain1;
        private string _domain2;
        private string _serverLogin1;
        private string _serverLogin2;
        private string _serverPassword1;
        private string _serverPassword2;
        private string _adminGroup1;
        private string _adminGroup2;
        private string _ou1;
        private string _ou2;

        private List<User> _userList1;
        private List<User> _userList2;
        private List<User> _mergedUserList;
        private List<User> _displayedUserList;

        private string _userList1Path;
        private string _userList2Path;
        private string _mergedUserListPath;


        public DisplayAccounts()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            _userList1 = new List<User>();
            _userList2 = new List<User>();
            _mergedUserList = new List<User>();
            _displayedUserList = new List<User>();

            _settings = new Settings();
            _login = new ServerAndAdminLogin();

            // Define the path to the repository
            _repositoryFilesPath = "C:\\ADsFusion\\UsersLists";

            // Check and create the repository directory if it doesn't exist
            CheckAndCreateDirectory(_repositoryFilesPath);

            // Save the path of the files
            _userList1Path = Path.Combine(_repositoryFilesPath, "UserList1.json");
            _userList2Path = Path.Combine(_repositoryFilesPath, "UserList2.json");
            _mergedUserListPath = Path.Combine(_repositoryFilesPath, "MergedUserList.json");

            // Define the list of files with their paths and default content
            List<(string filePath, string defaultContent)> files = new List<(string filePath, string defaultContent)>
            {
                (_userList1Path, ""),
                (_userList2Path, ""),
                (_mergedUserListPath, ""),
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
            DisplayUserList();
        }

        private void SetUserListFromJson(int x)
        {
            switch (x)
            {
                case 0:
                    break;
                case 1:
                    if (File.Exists(_userList1Path))
                    {
                        _displayedUserList = ReadFromJson(_userList1Path);
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _displayedUserList = new List<User>();
                    }
                    break;
                case 2:
                    if (File.Exists(_userList2Path))
                    {
                        _displayedUserList = ReadFromJson(_userList2Path);
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _displayedUserList = new List<User>();
                    }
                    break;
                case 3:
                    if (File.Exists(_mergedUserListPath))
                    {
                        _displayedUserList = ReadFromJson(_mergedUserListPath);
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _displayedUserList = new List<User>();
                    }
                    break;
            }
        }

        private int CheckIfLogged()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Domain1) && string.IsNullOrEmpty(Properties.Settings.Default.Domain2))
            {
                label2.Visible = true;
                return 0;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Domain1) && string.IsNullOrEmpty(Properties.Settings.Default.Domain2))
            {
                label2.Visible = false;
                UpdateLastUpdateTime(_userList1Path);
                return 1;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Domain2) && string.IsNullOrEmpty(Properties.Settings.Default.Domain1))
            {
                label2.Visible = false;
                UpdateLastUpdateTime(_userList2Path);
                return 2;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Domain1) && !string.IsNullOrEmpty(Properties.Settings.Default.Domain2))
            {
                label2.Visible = false;
                UpdateLastUpdateTime(_mergedUserListPath);
                return 3;
            }
            return 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length >= 3)
            {
                DisplayUserList();
            }
        }

        private void DisplayUserList()
        {
            SetUserListFromJson(CheckIfLogged());

            listBox1.Items.Clear();

            string searchText = textBox1.Text.Normalize().Trim().ToLower();

            foreach (User user in _displayedUserList)
            {
                // Check if SAMAccountName1 or SAMAccountName2 contains the search text (case-insensitive partial match)
                if ((!string.IsNullOrEmpty(user.SAMAccountName1) && user.SAMAccountName1.ToLower().Contains(searchText)) || !string.IsNullOrEmpty(user.SAMAccountName1) && searchText.Contains(user.SAMAccountName1.ToLower()) ||
                    (!string.IsNullOrEmpty(user.SAMAccountName2) && user.SAMAccountName2.ToLower().Contains(searchText)) || !string.IsNullOrEmpty(user.SAMAccountName2) && searchText.Contains(user.SAMAccountName2.ToLower()) ||
                    (!string.IsNullOrEmpty(user.DisplayName1) && user.DisplayName1.ToLower().Contains(searchText)) || !string.IsNullOrEmpty(user.DisplayName1) && searchText.Contains(user.DisplayName1.ToLower()) ||
                    (!string.IsNullOrEmpty(user.DisplayName2) && user.DisplayName2.ToLower().Contains(searchText)) || !string.IsNullOrEmpty(user.DisplayName2) && searchText.Contains(user.DisplayName2.ToLower()))
                {
                    // Add the user's SAMAccountName1 and SAMAccountName2 to the list box, if available
                    string displayText = $"{user.SAMAccountName1 ?? "N/A"} / {user.SAMAccountName2 ?? "N/A"}";
                    listBox1.Items.Add(displayText);
                }
            }
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
            UpdateAllAsync(CheckIfLogged());
        }

        private async Task UpdateAllAsync(int x)
        {
            _domain1 = Properties.Settings.Default.Domain1;
            _domain2 = Properties.Settings.Default.Domain2;
            _serverLogin1 = Properties.Settings.Default.Username1;
            _serverLogin2 = Properties.Settings.Default.Username2;
            _serverPassword1 = Properties.Settings.Default.Password1;
            _serverPassword2 = Properties.Settings.Default.Password2;
            _adminGroup1 = Properties.Settings.Default.Group1;
            _adminGroup2 = Properties.Settings.Default.Group2;
            _ou1 = Properties.Settings.Default.OU1;
            _ou2 = Properties.Settings.Default.OU2;

            switch (x)
            {
                case 0:
                    _login.ShowDialog();
                    break;
                case 1:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserList(_userList1, _userList1Path, _domain1, _ou1, 1));
                    progressBar1.Visible = false;
                    DisplayUserList();
                    break;
                case 2:
                    progressBar1.Visible = true;
                    _userList2 = await Task.Run(() => UpdateUserList(_userList2, _userList2Path, _domain2, _ou2, 2));
                    progressBar1.Visible = false;
                    DisplayUserList();
                    break;
                case 3:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserList(_userList1, _userList1Path, _domain1, _ou1, 1));
                    _userList2 = await Task.Run(() => UpdateUserList(_userList2, _userList2Path, _domain2, _ou2, 2));
                    progressBar1.Visible = false;
                    MergeUserList(); // No need to run in the background, as it's not a lengthy operation.
                    DisplayUserList();
                    break;
            }
        }

        private List<User> UpdateUserList(List<User> userlist, string userListPath, string domain, string ou, int selectedList)
        {
            // Create an empty list to store the active users.
            List<User> ActiveUsersAD = new List<User>();

            // Check if the JSON file exists and delete it to start with a fresh file.
            if (File.Exists(userListPath))
            {
                File.Delete(userListPath);
            }

            // Create a PrincipalSearcher and specify the UserPrincipal as the type to search for.
            var principalSearcher = new PrincipalSearcher(new UserPrincipal(new PrincipalContext(ContextType.Domain, domain)));
            if (!string.IsNullOrEmpty(ou))
            {
                principalSearcher = new PrincipalSearcher(new UserPrincipal(new PrincipalContext(ContextType.Domain, domain, ou)));
            }
            
            // Perform the search and get a collection of UserPrincipal objects.
            var userPrincipals = principalSearcher.FindAll();

            var progressCounter = 0;
            var totalUsers = userPrincipals.Count();

            // Define the batch size (e.g., 10 users at a time).
            const int batchSize = 10;

            // Loop through the collection of UserPrincipal objects to process each user.
            foreach (UserPrincipal userPrincipal in userPrincipals)
            {
                // Check if the user is active in Active Directory.
                if (userPrincipal != null && userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value && userPrincipal.SamAccountName != null && userPrincipal.EmailAddress != null)
                {
                    // The user is active. You can now proceed to retrieve user data and create User objects.
                    var groupsMembership = userPrincipal.GetGroups();
                    List<string> groups = new List<string>();
                    foreach (var groupMembership in groupsMembership)
                    {
                        groups.Add(groupMembership.Name);
                    }

                    // Get the underlying DirectoryEntry object.
                    var de = userPrincipal.GetUnderlyingObject() as DirectoryEntry;

                    User userToAdd = new User(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

                    if (selectedList == 1)
                    {
                        userToAdd = new User(
                            Convert.ToString(userPrincipal.SamAccountName),
                            Convert.ToString(userPrincipal.DisplayName),
                            Convert.ToString(userPrincipal.GivenName),
                            Convert.ToString(userPrincipal.Surname),
                            Convert.ToString(userPrincipal.EmailAddress),
                            Convert.ToString(de.Properties["extensionAttribute2"].Value?.ToString()),
                            Convert.ToString(userPrincipal.Description),
                            groups,
                            null, null, null, null, null, null, null, null);

                    }
                    if (selectedList == 2)
                    {
                        userToAdd = new User(
                        null, null, null, null, null, null, null, null,
                        Convert.ToString(userPrincipal.SamAccountName),
                        Convert.ToString(userPrincipal.DisplayName),
                        Convert.ToString(userPrincipal.GivenName),
                        Convert.ToString(userPrincipal.Surname),
                        Convert.ToString(userPrincipal.EmailAddress),
                        Convert.ToString(de.Properties["extensionAttribute2"].Value?.ToString()),
                        Convert.ToString(userPrincipal.Description),
                        groups);
                    }

                    // Add the user to the list of active users.
                    ActiveUsersAD.Add(userToAdd);

                    // Check if the batch size is reached or if we processed all users.
                    if (ActiveUsersAD.Count % batchSize == 0 || progressCounter == totalUsers - 1)
                    {
                        // Save the current batch to the JSON file.
                        SaveToJson(ActiveUsersAD, userListPath);

                        // Clear the list to free up memory for the next batch.
                        ActiveUsersAD.Clear();
                    }
                }
                progressCounter++;
                // Update the progress bar on the UI thread.
                this.Invoke(new Action(() =>
                {
                    progressBar1.Value = (int)((double)progressCounter / totalUsers * 100);
                }));
            }

            // Read the data back from the JSON file into ActiveUsersAD.
            List<User> userListToReturn = ReadFromJson(userListPath);
            return userListToReturn;
        }

        private void MergeUserList()
        {
            if (!string.IsNullOrEmpty(Properties.CustomNames.Default.MergeParameter))
            {
                string matchingParameter = Properties.CustomNames.Default.MergeParameter;

                foreach (User user1 in _userList1)
                {
                    string matchingValue1 = SelectMatchingValue(user1, matchingParameter);
                    User matchingUser = null;

                    foreach (User user2 in _userList2)
                    {
                        string matchingValue2 = SelectMatchingValue(user2, matchingParameter);
                        if (matchingValue1.Equals(matchingValue2))
                        {
                            matchingUser = user2;
                            break; // Stop searching once a match is found to avoid unnecessary iterations
                        }
                    }                    

                    if (matchingUser != null)
                    {
                        User mergedUser = new User(user1.SAMAccountName1, user1.DisplayName1, user1.GivenName1, user1.Sn1, user1.Mail1, user1.Title1, user1.Description1, user1.UserGroups1, matchingUser.SAMAccountName2, matchingUser.DisplayName2, matchingUser.GivenName2, matchingUser.Sn2, matchingUser.Mail2, matchingUser.Title2, matchingUser.Description2, matchingUser.UserGroups2);

                        _mergedUserList.Add(mergedUser);
                    }
                    else
                    {
                        _mergedUserList.Add(user1);
                    }
                }

                // Now, add all users from _userList2 that are not merged
                foreach (User user2 in _userList2)
                {
                    string matchingValue2 = SelectMatchingValue(user2, matchingParameter);

                    bool isMerged = _mergedUserList.Any(mergedUser =>
                        SelectMatchingValue(mergedUser, matchingParameter).Equals(matchingValue2));

                    if (!isMerged)
                    {
                        // Create a new User instance with properties from list2
                        User newUser = new User(null, null, null, null, null, null, null, null, user2.SAMAccountName2, user2.DisplayName2, user2.GivenName2, user2.Sn2, user2.Mail2, user2.Title2, user2.Description2, user2.UserGroups2);

                        _mergedUserList.Add(newUser);
                    }
                }

                // Save the merged list to a JSON file
                SaveToJson(_mergedUserList, _mergedUserListPath);
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner quel sera l'attribut commun afin de poursuivre la fusion des deux listes d'utilisateurs");
                _settings.ShowDialog();
            }
        }

        private string SelectMatchingValue(User user, string matchingParameter)
        {
            if (user.SAMAccountName1 != null)
            {
                switch (matchingParameter)
                {
                    case "SAMAccountName":
                        return user.SAMAccountName1;
                    case "DisplayName":
                        return user.DisplayName1;
                    case "GivenName":
                        return user.GivenName1;
                    case "Sn":
                        return user.Sn1;
                    case "Mail":
                        return user.Mail1;
                    case "Title":
                        return user.Title1;
                    case "Description":
                        return user.Description1;
                }
            }
            if (user.SAMAccountName2 != null)
            {
                switch (matchingParameter)
                {
                    case "SAMAccountName":
                        return user.SAMAccountName2;
                    case "DisplayName":
                        return user.DisplayName2;
                    case "GivenName":
                        return user.GivenName2;
                    case "Sn":
                        return user.Sn2;
                    case "Mail":
                        return user.Mail2;
                    case "Title":
                        return user.Title2;
                    case "Description":
                        return user.Description2;
                }
            }
            return null;
        }

        private void SaveToJson(List<User> users, string path)
        {
            string filePath = path;
            JsonManager.SaveToJson(users, filePath);
        }

        private List<User> ReadFromJson(string path)
        {
            string filePath = path;
            List<User> users = JsonManager.ReadFromJson(filePath);
            // Now, 'users' will contain the list of User objects from the JSON file.
            return users;
        }

        private void UpdateLastUpdateTime(string filePath)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(filePath);
            DateTime localTime = lastWriteTime.ToLocalTime();
            label1.Text = "Last update: " + localTime.ToString("dd.MM.yyyy HH:mm:ss");
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
