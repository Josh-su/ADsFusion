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
using System.Reflection;
using System.DirectoryServices.ActiveDirectory;

namespace ADsFusion
{
    public partial class DisplayAccounts : Form
    {
        private Settings _settings;
        private ServerAndAdminLogin _login;

        private int _selectedListBoxIndex;

        private string _repositoryUserListsFilesPath;
        private string _repositoryGroupsListsListsPath;

        private string _domain1;
        private string _domain2;
        private string _serverLogin1;
        private string _serverLogin2;
        private string _serverPassword1;
        private string _serverPassword2;
        private string _adminGroup1;
        private string _adminGroup2;

        private List<string> _groupList1;
        private List<string> _groupList2;
        private List<string> _allGroupsList;

        private List<User> _userList1;
        private List<User> _userList2;
        private List<User> _mergedUserList;
        private List<User> _actualUserList;
        private List<User> _filteredUserList;

        private string _userList1Path;
        private string _userList2Path;
        private string _mergedUserListPath;
        private string _groupListPath;

        private readonly object activeUsersLock = new object(); // Lock object
        List<Task> userTasks = new List<Task>();

        public DisplayAccounts()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            _userList1 = new List<User>();
            _userList2 = new List<User>();
            _mergedUserList = new List<User>();
            _actualUserList = new List<User>();
            _filteredUserList = new List<User>();

            _groupList1 = new List<string>();
            _groupList2 = new List<string>();
            _allGroupsList = new List<string>();

            _settings = new Settings();
            _login = new ServerAndAdminLogin();

            // Define the path to the repository
            _repositoryUserListsFilesPath = "C:\\ADsFusion\\UsersLists";
            _repositoryGroupsListsListsPath = "C:\\ADsFusion\\GroupsLists";

            // Check and create the repository directory if it doesn't exist
            CheckAndCreateDirectory(_repositoryUserListsFilesPath);
            CheckAndCreateDirectory(_repositoryGroupsListsListsPath);

            // Save the path of the files
            _userList1Path = Path.Combine(_repositoryUserListsFilesPath, "UserList1.json");
            _userList2Path = Path.Combine(_repositoryUserListsFilesPath, "UserList2.json");
            _mergedUserListPath = Path.Combine(_repositoryUserListsFilesPath, "MergedUserList.json");
            _groupListPath = Path.Combine(_repositoryGroupsListsListsPath, "GroupList.json");

            // Define the list of files with their paths and default content
            List<(string filePath, string defaultContent)> files = new List<(string filePath, string defaultContent)>
            {
                (_userList1Path, ""),
                (_userList2Path, ""),
                (_mergedUserListPath, ""),
                (_groupListPath, ""),
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
            UpdateFilteredUserList();
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
                        _actualUserList = ReadUsersFromJson(_userList1Path);
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _actualUserList = new List<User>();
                    }
                    break;
                case 2:
                    if (File.Exists(_userList2Path))
                    {
                        _actualUserList = ReadUsersFromJson(_userList2Path);
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _actualUserList = new List<User>();
                    }
                    break;
                case 3:
                    if (File.Exists(_mergedUserListPath))
                    {
                        _actualUserList = ReadUsersFromJson(_mergedUserListPath);
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _actualUserList = new List<User>();
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
            DisplayUserList();
        }

        private void DisplayUserList()
        {
            SetUserListFromJson(CheckIfLogged());

            listBox1.Items.Clear();

            string searchText = textBox1.Text.Normalize().Trim().ToLower(); // Convert to lowercase once

            foreach (User user in _filteredUserList)
            {
                string samAccountName1 = user.SAMAccountName1 ?? "N/A";
                string samAccountName2 = user.SAMAccountName2 ?? "N/A";
                string displayName1 = user.DisplayName1 ?? "";
                string displayName2 = user.DisplayName2 ?? "";

                // Convert the attributes to lowercase for case-insensitive comparisons
                samAccountName1 = samAccountName1.ToLower();
                samAccountName2 = samAccountName2.ToLower();
                displayName1 = displayName1.ToLower();
                displayName2 = displayName2.ToLower();

                if (samAccountName1.Contains(searchText) || samAccountName2.Contains(searchText) ||
                    displayName1.Contains(searchText) || displayName2.Contains(searchText))
                {
                    // Add the user's SAMAccountName1 and SAMAccountName2 to the list box, if available
                    string displayText = $"{samAccountName1} / {samAccountName2}";
                    listBox1.Items.Add(displayText);
                }
            }
        }

        private void CheckAndCreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                // Create the directory
                Directory.CreateDirectory(directoryPath);
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
            //_filterForm.Show(button1, new Point(0, button1.Height));

            UpdateFilteredUserList();
        }

        private void UpdateFilteredUserList()
        {
            _filteredUserList.Clear(); // Clear the existing filtered list

            _filteredUserList = _actualUserList;

            // Remove duplicate users (if any) and update the display
            _filteredUserList = _filteredUserList.Distinct().ToList();
            DisplayUserList();
        }
        #endregion

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listBox1.IndexFromPoint(e.Location);
                if (index >= 0 && index < listBox1.Items.Count)
                {
                    listBox1.SelectedIndex = index; // Ensure the right-clicked item is selected
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
            _groupList1 = Properties.Settings.Default.OU1.Split('|').ToList();
            _groupList2 = Properties.Settings.Default.OU2.Split('|').ToList();

            switch (x)
            {
                case 0:
                    _login.ShowDialog();
                    break;
                case 1:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserList(_userList1Path, _domain1, _groupList1, 1));
                    _userList1 = _userList1.Distinct().ToList();
                    UpdateGroupsListAndSaveToJson(_userList1, _groupListPath);
                    break;
                case 2:
                    progressBar1.Visible = true;
                    _userList2 = await Task.Run(() => UpdateUserList(_userList2Path, _domain2, _groupList2, 2));
                    _userList2 = _userList2.Distinct().ToList();
                    UpdateGroupsListAndSaveToJson(_userList2, _groupListPath);
                    break;
                case 3:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserList(_userList1Path, _domain1, _groupList1, 1));
                    _userList2 = await Task.Run(() => UpdateUserList(_userList2Path, _domain2, _groupList2, 2));
                    _userList1 = _userList1.Distinct().ToList();
                    _userList2 = _userList2.Distinct().ToList();
                    MergeUserList(); // No need to run in the background, as it's not a lengthy operation.
                    UpdateGroupsListAndSaveToJson(_mergedUserList, _groupListPath);
                    break;
            }
            if (x != 0)
            {
                progressBar1.Visible = false;
                UpdateFilteredUserList();
                DisplayUserList();
            }
        }

        private List<User> UpdateUserList(string userListPath, string domain, List<string> groupList, int selectedList)
        {
            // Create an empty list to store the active users.
            List<User> ActiveUsersAD = new List<User>();

            // Check if the JSON file exists and delete it to start with a fresh file.
            if (File.Exists(userListPath))
            {
                File.Delete(userListPath);
            }

            foreach (string group in groupList)
            {
                if (!string.IsNullOrEmpty(group))
                {
                    GetADUsers(group, selectedList, ActiveUsersAD, userListPath, domain);
                }
            }

            // Wait for all the tasks to complete before reading from the JSON file.
            Task.WhenAll(userTasks).Wait();

            // Read the data back from the JSON file into ActiveUsersAD.
            List<User> userListToReturn = ReadUsersFromJson(userListPath);
            return userListToReturn;
        }

        private void GetADUsers(string groupName, int selectedList, List<User> ActiveUsersAD, string userListPath, string domain)
        {
            var progressCounter = 0;
            // Define the batch size (e.g., 10 users at a time).
            const int batchSize = 10;

            // Create the PrincipalContext for the domain.
            var context = new PrincipalContext(ContextType.Domain, domain);

            // Find the group by its name.
            using (var groupPrincipal = GroupPrincipal.FindByIdentity(context, IdentityType.Name, groupName))
            {
                if (groupPrincipal != null)
                {
                    // Get the members of the group.
                    var groupMembers = groupPrincipal.GetMembers();

                    // Get the count of group members.
                    var totalMembers = groupMembers.Count();

                    // Loop through the group members to process each user.
                    foreach (var member in groupMembers)
                    {
                        if (member is UserPrincipal userPrincipal)
                        {
                            // Check if the user is active in Active Directory.
                            if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value &&
                                userPrincipal.SamAccountName != null && userPrincipal.EmailAddress != null)
                            {
                                userTasks.Add(Task.Run(async () =>
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

                                    // Add the user to the list of active users within a lock
                                    lock (activeUsersLock)
                                    {
                                        ActiveUsersAD.Add(userToAdd);

                                        // Check if the batch size is reached or if we processed all users.
                                        if (ActiveUsersAD.Count % batchSize == 0 || progressCounter == totalMembers - 1)
                                        {
                                            // Save the current batch to the JSON file.
                                            SaveUsersToJson(ActiveUsersAD, userListPath);

                                            // Clear the list to free up memory for the next batch.
                                            ActiveUsersAD.Clear();
                                        }
                                    }
                                    progressCounter++;
                                    // Update the progress bar on the UI thread.
                                    this.Invoke(new Action(() =>
                                    {
                                        progressBar1.Value = (int)((double)progressCounter / totalMembers * 100);
                                    }));
                                }));
                            }
                        }
                    }
                }
            }
        }

        private void UpdateGroupsListAndSaveToJson(List<User> userList, string groupListPath)
        {
            _allGroupsList.Clear();

            foreach (User user in userList)
            {
                if(user.UserGroups1 != null)
                {
                    foreach (string group in user.UserGroups1)
                    {
                        _allGroupsList.Add(group);
                    }
                }
                if (user.UserGroups2 != null)
                {
                    foreach (string group in user.UserGroups2)
                    {
                        _allGroupsList.Add(group);
                    }
                }
            }

            // Remove duplicate group names by applying Distinct() and updating _groupList.
            _allGroupsList = _allGroupsList.Distinct().ToList();

            // Save the list of group names to a JSON file.
            SaveGroupNamesToJson(_allGroupsList, groupListPath);
        }

        private void MergeUserList()
        {
            // Check if the JSON file exists and delete it to start with a fresh file.
            if (File.Exists(_mergedUserListPath))
            {
                File.Delete(_mergedUserListPath);
            }
            _mergedUserList.Clear();

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
                            break; // Stop searching once a match is found, to avoid unnecessary iterations
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
                SaveUsersToJson(_mergedUserList, _mergedUserListPath);
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
                        return user.SAMAccountName1.ToLower();
                    case "DisplayName":
                        return user.DisplayName1.ToLower();
                    case "GivenName":
                        return user.GivenName1.ToLower();
                    case "Sn":
                        return user.Sn1.ToLower();
                    case "Mail":
                        return user.Mail1.ToLower();
                    case "Title":
                        return user.Title1.ToLower();
                    case "Description":
                        return user.Description1.ToLower();
                }
            }
            if (user.SAMAccountName2 != null)
            {
                switch (matchingParameter)
                {
                    case "SAMAccountName":
                        return user.SAMAccountName2.ToLower();
                    case "DisplayName":
                        return user.DisplayName2.ToLower();
                    case "GivenName":
                        return user.GivenName2.ToLower();
                    case "Sn":
                        return user.Sn2.ToLower();
                    case "Mail":
                        return user.Mail2.ToLower();
                    case "Title":
                        return user.Title2.ToLower();
                    case "Description":
                        return user.Description2.ToLower();
                }
            }
            return null;
        }

        private void SaveUsersToJson(List<User> users, string path)
        {
            JsonManager.SaveToJson(users, path);
        }

        private List<User> ReadUsersFromJson(string path)
        {
            List<User> users = JsonManager.ReadFromJson<User>(path);
            // Now, 'users' will contain the list of User objects from the JSON file.
            return users;
        }

        private void SaveGroupNamesToJson(List<string> groupNames, string path)
        {
            JsonManager.SaveToJson(groupNames, path);
        }

        private List<string> ReadGroupNamesFromJson(string path)
        {
            List<string> loadedGroupNames = JsonManager.ReadFromJson<string>(path);
            // Now, 'loadedGroupNames' will contain the list of group names from the JSON file.
            return loadedGroupNames;
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

        #region Open Detail forms
        // Define a dictionary to store instances of AccountDetails forms.
        private Dictionary<int, AccountDetails> _accountDetailsForms = new Dictionary<int, AccountDetails>();

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenDetailsForm();
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenDetailsForm();
                e.Handled = true; // Prevent ListBox default behavior for Enter key
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
                OpenDetailsForm();
        }

        private void OpenDetailsForm()
        {
            foreach (int index in listBox1.SelectedIndices)
            {
                string displayText = listBox1.Items[index].ToString(); // Get the display text from the selected item
                User selectedUser = _actualUserList.FirstOrDefault(user =>
                {
                    string userDisplayText = $"{user.SAMAccountName1 ?? "N/A"} / {user.SAMAccountName2 ?? "N/A"}";
                    return userDisplayText.ToLower() == displayText;
                });

                if (selectedUser != null)
                {
                    // Check if a form for this index already exists.
                    if (!_accountDetailsForms.ContainsKey(index))
                    {
                        AccountDetails newForm = new AccountDetails();
                        newForm.InitializeWithUser(selectedUser); // Pass the selected user to the form

                        _accountDetailsForms.Add(index, newForm);
                        newForm.FormClosed += (s, args) => _accountDetailsForms.Remove(index);
                    }

                    // Show the form, whether it's a new instance or an existing one.
                    if (_accountDetailsForms.ContainsKey(index))
                    {
                        _accountDetailsForms[index].Show();
                        _accountDetailsForms[index].BringToFront();
                    }
                }
            }
        }
        #endregion
    }
}
