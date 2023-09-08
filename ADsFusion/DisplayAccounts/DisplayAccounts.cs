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
        private FilterForm _filterForm;

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
        private bool _isUserListsMerged = true; // Initial state is merged

        private string _userList1Path;
        private string _userList2Path;
        private string _mergedUserListPath;
        private string _groupListPath;

        // Define dictionarys to store instances of AccountDetails forms.
        private Dictionary<User, MergedAccountDetails> _mergedAccountsDetailsForms;
        private Dictionary<User, SingleAccountDetails> _singleAccountsDetailsForms;

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
            _filterForm = new FilterForm();

            _mergedAccountsDetailsForms = new Dictionary<User, MergedAccountDetails>();
            _singleAccountsDetailsForms = new Dictionary<User, SingleAccountDetails>();

            // Attach the LocationChanged event handler
            this.LocationChanged += DisplayAccounts_LocationChanged;

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

        private void DisplayAccounts_LocationChanged(object sender, EventArgs e)
        {
            // Update the location of the FilterForm to be on the left of DisplayAccounts
            _filterForm.Location = new Point(this.Left - _filterForm.Width, this.Top);
        }

        #region check if the initial files exist if not create them
        private void CheckAndCreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                // Create the directory
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="defaultContent"></param>
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
        #endregion

        private void DisplayAccounts_Load(object sender, EventArgs e)
        {
            UpdateAllAsync(CheckIfLogged());
        }

        private void SetUserListFromJson()
        {
            if (File.Exists(_userList1Path))
            {
                _userList1 = ReadUsersFromJson(_userList1Path);
                _userList1 = _userList1.Distinct().ToList();
            }
            else
            {
                // The file doesn't exist or is empty, create an empty list
                _userList1 = new List<User>();
            }
            if (File.Exists(_userList2Path))
            {
                _userList2 = ReadUsersFromJson(_userList2Path);
                _userList2 = _userList2.Distinct().ToList();
            }
            else
            {
                // The file doesn't exist or is empty, create an empty list
                _userList2 = new List<User>();
            }
            if (File.Exists(_mergedUserListPath))
            {
                _mergedUserList = ReadUsersFromJson(_mergedUserListPath);
                _mergedUserList = _mergedUserList.Distinct().ToList();
            }
            else
            {
                // The file doesn't exist or is empty, create an empty list
                _mergedUserList = new List<User>();
            }

            UpdateActualUserList();
            UpdateGroupsListAndSaveToJson(_actualUserList, _groupListPath);
            _allGroupsList = ReadGroupNamesFromJson(_groupListPath);
            _filterForm.ListGroups.Clear();
            _filterForm.ListGroups = _allGroupsList;
            _filterForm.UpdateGroups();
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
                    if (_isUserListsMerged)
                    {
                        string displayText = $"{samAccountName1} / {samAccountName2}";
                        AddItemToListBox(displayText);
                    }
                    else
                    {
                        string displayText = $"{samAccountName1 ?? samAccountName2}, {displayName1 ?? displayName2}";
                        AddItemToListBox(displayText);
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            _login.ShowDialog();
        }

        /// <summary>
        /// Show the Settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            _settings.ShowDialog();
            /*MergeUserList();
            SetUserListFromJson();*/
        }

        #region Account list filter
        private void button1_Click(object sender, EventArgs e)
        {
            if (!_filterForm.Visible)
            {
                _filterForm.Show();
            }

            // Update your user interface or perform any other necessary actions
            UpdateFilteredUserList(_filterForm.SelectedGroups);
            DisplayUserList();
        }

        private void UpdateFilteredUserList(List<string> groups)
        {
            _filteredUserList.Clear(); // Clear the existing filtered list

            if(groups != null && groups.Count > 0)
            {
                // Iterate through each user in _actualUserList
                foreach (var user in _actualUserList)
                {
                    // Check if any group in UserGroups1 or UserGroups2 exists in the provided 'groups' list
                    bool hasMatchingGroup = false;
                    if (user.UserGroups1?.Intersect(groups).Any() ?? false)
                    {
                        hasMatchingGroup = true;
                    }
                    if (user.UserGroups2?.Intersect(groups).Any() ?? false)
                    {
                        hasMatchingGroup = true;
                    }

                    if (hasMatchingGroup)
                    {
                        _filteredUserList.Add(user);
                    }
                }
            }
            else
            {
                _filteredUserList = _actualUserList;
            }

            // Remove duplicate users (if any) and update the display
            _filteredUserList = _filteredUserList.Distinct().ToList();

            DisplayUserList();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #region Update Users Lists
        private void button2_Click(object sender, EventArgs e)
        {
            UpdateAllAsync(CheckIfLogged());
        }

        private async Task UpdateAllAsync(int x)
        {
            _domain1 = Properties.Settings.Default.Domain1;
            _domain2 = Properties.Settings.Default.Domain2;
            /*
            _serverLogin1 = Properties.Settings.Default.Username1;
            _serverLogin2 = Properties.Settings.Default.Username2;
            _serverPassword1 = Properties.Settings.Default.Password1;
            _serverPassword2 = Properties.Settings.Default.Password2;
            _adminGroup1 = Properties.Settings.Default.GroupAdmin1;
            _adminGroup2 = Properties.Settings.Default.GroupAdmin2;
            */
            _groupList1 = Properties.Settings.Default.Groups1.Split('|').ToList();
            _groupList1.RemoveAll(group => string.IsNullOrWhiteSpace(group)); // remove all the empty entry
            _groupList2 = Properties.Settings.Default.Groups2.Split('|').ToList();
            _groupList2.RemoveAll(group => string.IsNullOrWhiteSpace(group)); // remove all the empty entry

            switch (x)
            {
                case 0:
                    _login.ShowDialog();
                    _settings.ShowDialog();
                    UpdateAllAsync(CheckIfLogged());
                    break;
                case 1:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserList(_userList1Path, _domain1, _groupList1, 1));
                    //UpdateGroupsListAndSaveToJson(_userList1, _groupListPath);
                    break;
                case 2:
                    progressBar1.Visible = true;
                    _userList2 = await Task.Run(() => UpdateUserList(_userList2Path, _domain2, _groupList2, 2));
                    //UpdateGroupsListAndSaveToJson(_userList2, _groupListPath);
                    break;
                case 3:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserList(_userList1Path, _domain1, _groupList1, 1));
                    _userList2 = await Task.Run(() => UpdateUserList(_userList2Path, _domain2, _groupList2, 2));
                    MergeUserList();
                    //UpdateGroupsListAndSaveToJson(_mergedUserList, _groupListPath);
                    break;
            }
            if (x != 0)
            {
                progressBar1.Visible = false;
                SetUserListFromJson();
                UpdateFilteredUserList(_filterForm.SelectedGroups);
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
                                            SaveUsersToJson(ActiveUsersAD, userListPath, false);

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
            SaveGroupNamesToJson(_allGroupsList, groupListPath, true);
        }

        private void MergeUserList()
        {
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

                // Save the merged list to a JSON file, clearing existing content
                SaveUsersToJson(_mergedUserList, _mergedUserListPath, true);
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
        #endregion

        #region Json Manager
        private void SaveUsersToJson(List<User> users, string path, bool clear)
        {
            JsonManager.SaveToJson(users, path, clear);
        }

        private List<User> ReadUsersFromJson(string path)
        {
            List<User> users = JsonManager.ReadFromJson<User>(path);
            // Now, 'users' will contain the list of User objects from the JSON file.
            return users;
        }

        private void SaveGroupNamesToJson(List<string> groupNames, string path, bool clear)
        {
            JsonManager.SaveToJson(groupNames, path, clearExisting : clear);
        }

        private List<string> ReadGroupNamesFromJson(string path)
        {
            List<string> loadedGroupNames = JsonManager.ReadFromJson<string>(path);
            // Now, 'loadedGroupNames' will contain the list of group names from the JSON file.
            return loadedGroupNames;
        }
        #endregion

        /// <summary>
        /// update the date of the last update of the users lists
        /// </summary>
        /// <param name="filePath"></param>
        private void UpdateLastUpdateTime(string filePath)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(filePath);
            DateTime localTime = lastWriteTime.ToLocalTime();
            label1.Text = "Last update: " + localTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        /// <summary>
        /// Button Help that open the web page of the Github repository
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                User selectedUser = new User();
                if (_isUserListsMerged)
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? "N/A"} / {user.SAMAccountName2 ?? "N/A"}";
                        return userDisplayText.ToLower() == displayText;
                    });
                }
                else
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? user.SAMAccountName2}, {user.DisplayName1 ?? user.DisplayName2}";
                        return userDisplayText.ToLower() == displayText;
                    });
                }

                if (selectedUser != null)
                {
                    // Check if a form for this index already exists.
                    if(_isUserListsMerged && !_mergedAccountsDetailsForms.ContainsKey(selectedUser))
                    {
                        MergedAccountDetails newForm = new MergedAccountDetails();
                        newForm.InitializeWithUser(selectedUser); // Pass the selected user to the form

                        _mergedAccountsDetailsForms.Add(selectedUser, newForm);
                        newForm.FormClosed += (s, args) => _mergedAccountsDetailsForms.Remove(selectedUser);
                    }
                    else if (!_isUserListsMerged && !_singleAccountsDetailsForms.ContainsKey(selectedUser))
                    {
                        SingleAccountDetails newForm = new SingleAccountDetails();
                        newForm.InitializeWithUser(selectedUser); // Pass the selected user to the form

                        _singleAccountsDetailsForms.Add(selectedUser, newForm);
                        newForm.FormClosed += (s, args) => _singleAccountsDetailsForms.Remove(selectedUser);
                    }

                    // Show the form, whether it's a new instance or an existing one.
                    if (_isUserListsMerged && _mergedAccountsDetailsForms.ContainsKey(selectedUser))
                    {
                        _mergedAccountsDetailsForms[selectedUser].Show();
                        _mergedAccountsDetailsForms[selectedUser].BringToFront();
                    }
                    if (!_isUserListsMerged && _singleAccountsDetailsForms.ContainsKey(selectedUser))
                    {
                        _singleAccountsDetailsForms[selectedUser].Show();
                        _singleAccountsDetailsForms[selectedUser].BringToFront();
                    }
                }
            }
        }
        #endregion

        private void UpdateCountItemLabel()
        {
            int Totalcount = UpdateTotalCountItem();
            int SelectedCount = UpdateCountSelectedItem();

            label3.Text = "Selected items : " + SelectedCount + " | Total items : " + Totalcount;
        }

        private int UpdateTotalCountItem()
        {
            return listBox1.Items.Count;
        }

        private int UpdateCountSelectedItem()
        {
            return listBox1.SelectedItems.Count;
        }

        /// <summary>
        /// export the list in a csv file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // Create a StringBuilder to build the CSV content
                    StringBuilder csvContent = new StringBuilder();

                    foreach (var item in listBox1.Items)
                    {
                        // Extract SAMAccountName1 and SAMAccountName2 from the list item
                        string[] parts = item.ToString().Split(new string[] { " / " }, StringSplitOptions.None);
                        string samAccountName1 = parts[0].Trim();
                        string samAccountName2 = parts.Length > 1 ? parts[1].Trim() : "";

                        // Append the values to the CSV content
                        csvContent.AppendLine($"{samAccountName1},{samAccountName2}");
                    }

                    // Write the CSV content to the selected file
                    File.WriteAllText(filePath, csvContent.ToString());

                    MessageBox.Show("CSV file exported successfully.", "Export Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void UpdateActualUserList()
        {
            // Update the button image based on the state
            if (_isUserListsMerged)
            {
                button10.Image = Properties.Resources.split_20; // Set to split image
                _actualUserList = _mergedUserList; // Set _actualUserList to the merged list
            }
            else
            {
                button10.Image = Properties.Resources.merge_20; // Set to merge image
                // Set _actualUserList to the two separate lists, you may need to adjust this based on your data structure
                _actualUserList = _userList1.Concat(_userList2).ToList();
            }
        }

        /// <summary>
        /// merge or split the two users lists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            // Change the state
            _isUserListsMerged = !_isUserListsMerged;

            UpdateActualUserList();

            // Update your user interface or perform any other necessary actions
            UpdateFilteredUserList(_filterForm.SelectedGroups);
            DisplayUserList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void AddItemToListBox(string item)
        {
            listBox1.Items.Add(item); // Add an item to the ListBox
            UpdateCountItemLabel(); // Update the count label
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void RemoveItemFromListBox(string item)
        {
            listBox1.Items.Remove(item); // Remove an item from the ListBox
            UpdateCountItemLabel(); // Update the count label
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCountItemLabel();
        }

        /// <summary>
        /// imprimer un document avec les informations sur le compte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void impressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //printAccountInfo(_isUserListsMerged);
        }

        private void printAccountInfo(bool isUserListsMerged)
        {
            foreach (int index in listBox1.SelectedIndices)
            {
                string displayText = listBox1.Items[index].ToString(); // Get the display text from the selected item
                User selectedUser = new User();
                if (isUserListsMerged)
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? "N/A"} / {user.SAMAccountName2 ?? "N/A"}";
                        return userDisplayText.ToLower() == displayText;
                    });
                }
                else
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? "N/A"} / {user.SAMAccountName2 ?? "N/A"}";
                        return userDisplayText.ToLower() == displayText;
                    });
                }
            }
        }
    }
}
