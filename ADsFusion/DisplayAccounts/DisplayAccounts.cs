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
using ADsFusion;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ADsFusion
{
    public partial class DisplayAccounts : Form
    {
        private readonly Settings _settings;
        private readonly ServerAndAdminLogin _login;
        private readonly FilterForm _filterForm;

        // Define dictionarys to store instances of AccountDetails forms.
        private readonly Dictionary<User, MergedAccountDetails> _mergedAccountsDetailsForms;
        private readonly Dictionary<User, SingleAccountDetails> _singleAccountsDetailsForms;

        private readonly string _repositoryUserListsFilesPath;
        private readonly string _repositoryGroupsListsListsPath;

        private string _domain1;
        private string _domain2;

        /*
        private string _serverLogin1;
        private string _serverLogin2;
        private string _serverPassword1;
        private string _serverPassword2;
        private string _adminGroup1;
        private string _adminGroup2;*/

        private List<string> _groupList1;
        private List<string> _groupList2;
        private List<string> _allGroupsList;

        private List<User> _userList1;
        private List<User> _userList2;
        private List<User> _mergedUserList;
        private List<User> _actualUserList;
        private List<User> _filteredUserList;
        private bool _isUserListsMerged = true; // Initial state is merged

        private readonly string _userList1Path;
        private readonly string _userList2Path;
        private readonly string _mergedUserListPath;
        private readonly string _groupListPath;

        private bool _isBackgroundProcessRunning = false;
        private bool _printAccountInfoFlag = false; // Using the word "Flag" in the variable name is a common convention to indicate that it's a boolean flag used for controlling a specific behavior

        private readonly object activeUsersLock = new object(); // Lock object
        private readonly List<Task> _userTasks = new List<Task>();

        /// <summary>
        /// 
        /// </summary>
        public DisplayAccounts()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Set the MaximumSize and MinimumSize to the initial size of your form
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            // Enable KeyPreview
            this.KeyPreview = true;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayAccounts_LocationChanged(object sender, EventArgs e)
        {
            // Update the location of the FilterForm to be on the left of DisplayAccounts
            _filterForm.Location = new Point(this.Left - _filterForm.Width, this.Top);
        }

        #region check if the initial files exist if not create them
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DisplayAccounts_Load(object sender, EventArgs e)
        {
            await UpdateAllAsync(CheckIfLogged());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        private void SetUserListFromJson(int x)
        {
            switch (x)
            {
                case 1:
                    if (File.Exists(_userList1Path))
                    {
                        _userList1 = ReadUsersFromJson(_userList1Path);
                        _userList1 = _userList1.Distinct().ToList();
                        _userList2 = new List<User>();
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _userList1 = new List<User>();
                    }
                    break;
                case 2:
                    if (File.Exists(_userList2Path))
                    {
                        _userList2 = ReadUsersFromJson(_userList2Path);
                        _userList2 = _userList2.Distinct().ToList();
                        _userList1 = new List<User>();
                    }
                    else
                    {
                        // The file doesn't exist or is empty, create an empty list
                        _userList2 = new List<User>();
                    }
                    break;
                case 3:
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
                    break;
            }

            UpdateActualUserList();
            UpdateGroupsListAndSaveToJson(_actualUserList, _groupListPath);
            _allGroupsList = ReadGroupNamesFromJson(_groupListPath);
            _filterForm.ListGroups.Clear();
            _filterForm.ListGroups = _allGroupsList;
            _filterForm.UpdateGroups();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            DisplayUserList();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayUserList()
        {
            listBox1.Items.Clear();

            string searchText = textBox1.Text.Normalize().Trim().ToLower(); // Convert to lowercase once

            foreach (User user in _filteredUserList)
            {
                string samAccountName1 = user.SAMAccountName1;
                string samAccountName2 = user.SAMAccountName2;
                string displayName1 = user.DisplayName1;
                string displayName2 = user.DisplayName2;

                if (_isUserListsMerged && CheckIfLogged() == 3)
                {
                    string displayText = $"{(string.IsNullOrEmpty(samAccountName1) ? "n/a" : samAccountName1)} / {(string.IsNullOrEmpty(samAccountName2) ? "n/a" : samAccountName2)}";
                    if (displayText.Normalize().Trim().ToLower().Contains(searchText))
                    {
                        AddItemToListBox(displayText);
                    }
                }
                else
                {
                    List<string> parts = new List<string>();

                    if (!string.IsNullOrEmpty(samAccountName1))
                    {
                        parts.Add(samAccountName1);
                    }
                    if (!string.IsNullOrEmpty(samAccountName2))
                    {
                        parts.Add(samAccountName2);
                    }
                    if (!string.IsNullOrEmpty(displayName1))
                    {
                        parts.Add(displayName1);
                    }
                    if (!string.IsNullOrEmpty(displayName2))
                    {
                        parts.Add(displayName2);
                    }

                    string displayText = string.Join(", ", parts);

                    if (displayText.Normalize().Trim().ToLower().Contains(searchText))
                    {
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
        private void Button4_Click(object sender, EventArgs e)
        {
            _login.ShowDialog();
        }

        /// <summary>
        /// Show the Settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {
            _settings.ShowDialog();
            /*MergeUserList();
            SetUserListFromJson();*/
        }

        #region Account list filter
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            if (!_filterForm.Visible)
            {
                _filterForm.Show();
            }
            else
            {
                // Update your user interface
                UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
                DisplayUserList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="selectAllMatchingGroups"></param>
        private void UpdateFilteredUserList(List<string> groups, bool selectAllMatchingGroups)
        {
            _filteredUserList.Clear(); // Clear the existing filtered list

            if (groups != null && groups.Count > 0)
            {
                // Iterate through each user in _actualUserList
                foreach (var user in _actualUserList)
                {
                    bool hasMatchingGroup = false;
                    if (selectAllMatchingGroups)
                    {
                        // Combine UserGroups1 and UserGroups2 into a single list
                        List<string> allUserGroups = (user.UserGroups1 ?? new List<string>()).Concat(user.UserGroups2 ?? new List<string>()).ToList();

                        // Check if all groups in 'groups' are present in allUserGroups
                        hasMatchingGroup = groups.All(group => allUserGroups.Contains(group));

                        if (hasMatchingGroup)
                        {
                            _filteredUserList.Add(user);
                        }
                    }
                    else
                    {
                        // Check if any group in UserGroups1 or UserGroups2 exists in the provided 'groups' list
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
        private void ListBox1_MouseUp(object sender, MouseEventArgs e)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button2_Click(object sender, EventArgs e)
        {
            await UpdateAllAsync(CheckIfLogged());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
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
                    await UpdateAllAsync(CheckIfLogged());
                    break;
                case 1:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserListAsync(_userList1Path, _domain1, _groupList1, 1));
                    //UpdateGroupsListAndSaveToJson(_userList1, _groupListPath);
                    break;
                case 2:
                    progressBar1.Visible = true;
                    _userList2 = await Task.Run(() => UpdateUserListAsync(_userList2Path, _domain2, _groupList2, 2));
                    //UpdateGroupsListAndSaveToJson(_userList2, _groupListPath);
                    break;
                case 3:
                    progressBar1.Visible = true;
                    _userList1 = await Task.Run(() => UpdateUserListAsync(_userList1Path, _domain1, _groupList1, 1));
                    _userList2 = await Task.Run(() => UpdateUserListAsync(_userList2Path, _domain2, _groupList2, 2));
                    MergeUserList();
                    //UpdateGroupsListAndSaveToJson(_mergedUserList, _groupListPath);
                    break;
            }
            if (x != 0)
            {
                progressBar1.Visible = false;
                SetUserListFromJson(x);
                UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
                DisplayUserList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userListPath"></param>
        /// <param name="domain"></param>
        /// <param name="groupList"></param>
        /// <param name="selectedList"></param>
        /// <returns></returns>
        private async Task<List<User>> UpdateUserListAsync(string userListPath, string domain, List<string> groupList, int selectedList)
        {
            // Set the flag to true when the background process starts
            _isBackgroundProcessRunning = true;

            try // Your background process code here
            {
                // Create an empty list to store the active users.
                List<User> ActiveUsersAD = new List<User>();

                // Check if the JSON file exists and delete it to start with a fresh file.
                if (File.Exists(userListPath))
                {
                    File.Delete(userListPath);
                }

                Parallel.ForEach(groupList, groupName =>
                {
                    GetADUsers(groupName, selectedList, ActiveUsersAD, userListPath, domain);
                });

                // Wait for all the tasks to complete before reading from the JSON file.
                await Task.WhenAll(_userTasks);

                // Read the data back from the JSON file into ActiveUsersAD.
                List<User> userListToReturn = ReadUsersFromJson(userListPath);
                return userListToReturn;
            }
            finally
            {
                // Set the flag to false when the background process finishes
                _isBackgroundProcessRunning = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="selectedList"></param>
        /// <param name="ActiveUsersAD"></param>
        /// <param name="userListPath"></param>
        /// <param name="domain"></param>
        private void GetADUsers(string groupName, int selectedList, List<User> ActiveUsersAD, string userListPath, string domain)
        {
            var progressCounter = 0;
            // Define the batch size (e.g., 600 users at a time).
            const int batchSize = 600;

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

                    var totalActiveUserPrincipal = 0;

                    // Loop through the group members to process each user.
                    foreach (var member in groupMembers)
                    {
                        if (member is UserPrincipal userPrincipal)
                        {
                            // Check if the user is active in Active Directory.
                            if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value && userPrincipal.SamAccountName != null)
                            {
                                _userTasks.Add(Task.Run(() =>
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
                                        if (ActiveUsersAD.Count % batchSize == 0 || progressCounter == totalActiveUserPrincipal - 1)
                                        {
                                            // Save the current batch to the JSON file.
                                            SaveUsersToJson(ActiveUsersAD, userListPath, false);

                                            // Clear the list to free up memory for the next batch.
                                            ActiveUsersAD.Clear();
                                        }
                                    }
                                    // Update the progress bar on the UI thread.
                                    this.Invoke(new Action(() =>
                                    {
                                        progressBar1.Value = (int)((double)progressCounter / totalActiveUserPrincipal * 100);
                                    }));
                                    // Increment progressCounter safely
                                    Interlocked.Increment(ref progressCounter);
                                }));
                                totalActiveUserPrincipal++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="groupListPath"></param>
        private void UpdateGroupsListAndSaveToJson(List<User> userList, string groupListPath)
        {
            _allGroupsList.Clear();

            foreach (User user in userList)
            {

                if (user.UserGroups1 != null)
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

        /// <summary>
        /// 
        /// </summary>
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
                        if (matchingValue1 != null && matchingValue1.Equals(matchingValue2))
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
                    string matchingValue2 = (SelectMatchingValue(user2, matchingParameter));

                    bool isMerged = _mergedUserList.Any(mergedUser => SelectMatchingValue(mergedUser, matchingParameter).Equals(matchingValue2));

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
                MergeUserList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="matchingParameter"></param>
        /// <returns></returns>
        private string SelectMatchingValue(User user, string matchingParameter)
        {
            if (user.SAMAccountName1 != null)
            {
                switch (matchingParameter)
                {
                    case "SAMAccountName":
                        return user.SAMAccountName1?.ToLower() ?? string.Empty;
                    case "DisplayName":
                        return user.DisplayName1?.ToLower() ?? string.Empty;
                    case "GivenName":
                        return user.GivenName1?.ToLower() ?? string.Empty;
                    case "Sn":
                        return user.Sn1?.ToLower() ?? string.Empty;
                    case "Mail":
                        return user.Mail1?.ToLower() ?? string.Empty;
                    case "Title":
                        return user.Title1?.ToLower() ?? string.Empty;
                    case "Description":
                        return user.Description1?.ToLower() ?? string.Empty;
                }
            }
            if (user.SAMAccountName2 != null)
            {
                switch (matchingParameter)
                {
                    case "SAMAccountName":
                        return user.SAMAccountName2?.ToLower() ?? string.Empty;
                    case "DisplayName":
                        return user.DisplayName2?.ToLower() ?? string.Empty;
                    case "GivenName":
                        return user.GivenName2?.ToLower() ?? string.Empty;
                    case "Sn":
                        return user.Sn2?.ToLower() ?? string.Empty;
                    case "Mail":
                        return user.Mail2?.ToLower() ?? string.Empty;
                    case "Title":
                        return user.Title2?.ToLower() ?? string.Empty;
                    case "Description":
                        return user.Description2?.ToLower() ?? string.Empty;
                }
            }
            return string.Empty;
        }
        #endregion

        #region Json Manager
        /// <summary>
        /// 
        /// </summary>
        /// <param name="users"></param>
        /// <param name="path"></param>
        /// <param name="clear"></param>
        private void SaveUsersToJson(List<User> users, string path, bool clear)
        {
            JsonManager.SaveToJson(users, path, clear);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<User> ReadUsersFromJson(string path)
        {
            List<User> users = JsonManager.ReadFromJson<User>(path);
            // Now, 'users' will contain the list of User objects from the JSON file.
            return users;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupNames"></param>
        /// <param name="path"></param>
        /// <param name="clear"></param>
        private void SaveGroupNamesToJson(List<string> groupNames, string path, bool clear)
        {
            JsonManager.SaveToJson(groupNames, path, clearExisting: clear);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        private void Button6_Click(object sender, EventArgs e)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenDetailsForm();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenDetailsForm();
        }

        /// <summary>
        /// 
        /// </summary>
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
                        string userDisplayText = $"{user.SAMAccountName1 ?? "n/a"} / {user.SAMAccountName2 ?? "n/a"}";
                        return userDisplayText.ToLower() == displayText.ToLower();
                    });
                }
                else
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? user.SAMAccountName2}, {user.DisplayName1 ?? user.DisplayName2}";
                        return userDisplayText.ToLower() == displayText.ToLower();
                    });
                }

                if (selectedUser != null)
                {
                    // Check if a form for this index already exists.
                    if (_isUserListsMerged && !_mergedAccountsDetailsForms.ContainsKey(selectedUser))
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
                else
                {
                    MessageBox.Show("Tu n'as sélectionné aucun utilisateur !!");
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCountItemLabel()
        {
            int Totalcount = UpdateTotalCountItem();
            int SelectedCount = UpdateCountSelectedItem();

            label3.Text = "Selected items : " + SelectedCount + " | Total items : " + Totalcount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int UpdateTotalCountItem()
        {
            return listBox1.Items.Count;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int UpdateCountSelectedItem()
        {
            return listBox1.SelectedItems.Count;
        }

        /// <summary>
        /// export the list in a csv file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button8_Click(object sender, EventArgs e)
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

        /// <summary>
        /// 
        /// </summary>
        private void UpdateActualUserList()
        {
            if (CheckIfLogged() == 3)
            {
                button10.Enabled = true;
                // Update the button image based on the state
                if (_isUserListsMerged)
                {
                    button10.Image = Properties.Resources.split_20; // Set to split image
                    _actualUserList = _mergedUserList; // Set _actualUserList to the merged list
                }
                else
                {
                    button10.Image = Properties.Resources.merge_20; // Set to merge image
                    // Set _actualUserList to the two separate lists
                    _actualUserList = _userList1.Concat(_userList2).ToList();
                }
            }
            else
            {
                button10.Enabled = false;
                button10.Image = Properties.Resources.merge_20; // Set to merge image
                _actualUserList = _userList1.Concat(_userList2).ToList();
            }
        }

        /// <summary>
        /// merge or split the two users lists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button10_Click(object sender, EventArgs e)
        {
            // Change the state
            _isUserListsMerged = !_isUserListsMerged;

            UpdateActualUserList();

            // Update your user interface or perform any other necessary actions
            UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
            DisplayUserList();
        }

        /// <summary>
        /// ajoute un item dans listBox1 et met a jour le label avec le nombre d'item
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
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCountItemLabel();
        }

        /// <summary>
        /// imprimer un document avec les informations sur le compte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintAccountInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrintAccountInfo()
        {
            foreach (int index in listBox1.SelectedIndices)
            {
                string displayText = listBox1.Items[index].ToString(); // Get the display text from the selected item
                User selectedUser = new User();
                if (_isUserListsMerged)
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? "n/a"} / {user.SAMAccountName2 ?? "n/a"}";
                        return userDisplayText.ToLower() == displayText.ToLower();
                    });
                }
                else
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? user.SAMAccountName2}, {user.DisplayName1 ?? user.DisplayName2}";
                        return userDisplayText.ToLower() == displayText.ToLower();
                    });
                }

                if (selectedUser != null)
                {
                    //print logic...
                    MessageBox.Show(selectedUser.SAMAccountName1 + selectedUser.SAMAccountName2);


                }
                else
                {
                    MessageBox.Show("Tu n'as sélectionné aucun utilisateur !!");
                }
            }
        }

        #region Delete
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupprimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteUserAccount();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button9_Click(object sender, EventArgs e)
        {
            DeleteUserAccount();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteUserAccount()
        {
            foreach (int index in listBox1.SelectedIndices)
            {
                string displayText = listBox1.Items[index].ToString(); // Get the display text from the selected item
                User selectedUser = new User();
                if (_isUserListsMerged)
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? "n/a"} / {user.SAMAccountName2 ?? "n/a"}";
                        return userDisplayText.ToLower() == displayText.ToLower();
                    });
                }
                else
                {
                    selectedUser = _actualUserList.FirstOrDefault(user =>
                    {
                        string userDisplayText = $"{user.SAMAccountName1 ?? user.SAMAccountName2}, {user.DisplayName1 ?? user.DisplayName2}";
                        return userDisplayText.ToLower() == displayText.ToLower();
                    });
                }

                if (selectedUser != null)
                {
                    MessageBox.Show(selectedUser.SAMAccountName1 + " " + selectedUser.SAMAccountName2);
                }
                else
                {
                    MessageBox.Show("Tu n'as sélectionné aucun utilisateur !!");
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Ctrl key is pressed
            if (e.Control)
            {
                // Handle Ctrl+Up: Select the first item
                if (e.KeyCode == Keys.Up)
                {
                    if (listBox1.Items.Count > 0)
                    {
                        listBox1.SetSelected(0, true);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }

                // Handle Ctrl+Down: Select the last item
                if (e.KeyCode == Keys.Down)
                {
                    int lastIndex = listBox1.Items.Count - 1;
                    if (lastIndex >= 0)
                    {
                        listBox1.SetSelected(lastIndex, true);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }

                // Handle Ctrl+A for selecting all items
                if (e.KeyCode == Keys.A)
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        listBox1.SetSelected(i, true);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.P)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    _printAccountInfoFlag = true;
                    return;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Check if Alt key is pressed
            if (e.Alt)
            {
                // Handle Alt+M for merge user list
                if (e.KeyCode == Keys.M)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    if (!_isUserListsMerged)
                    {
                        // Change the state
                        _isUserListsMerged = true;

                        UpdateActualUserList();

                        // Update your user interface or perform any other necessary actions
                        UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
                        DisplayUserList();
                    }
                    return;
                }

                // Handle Alt+S for split user list
                if (e.KeyCode == Keys.S)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    if (_isUserListsMerged)
                    {
                        // Change the state
                        _isUserListsMerged = false;
                        UpdateActualUserList();

                        // Update your user interface or perform any other necessary actions
                        UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
                        DisplayUserList();
                    }
                    return;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Handle Enter key
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                e.SuppressKeyPress = true;

                OpenDetailsForm();

                return;
            }

            // Check if the pressed key is Escape (Esc)
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;

                e.SuppressKeyPress = true;

                // Display a confirmation dialog
                DialogResult result = MessageBox.Show("Are you sure you want to close the form?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Check the user's response
                if (result == DialogResult.Yes)
                {
                    // Close the form
                    this.Close();
                }
                return;
            }
        }

        /// <summary>
        /// Check if there a background process before closing the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayAccounts_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the background process is running
            if (_isBackgroundProcessRunning)
            {
                // Display a message or take appropriate action to inform the user
                MessageBox.Show("Please wait for the background process to finish \nbefore closing the application.");
                // Cancel the form closing event to prevent the application from closing
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (_printAccountInfoFlag)
            {
                PrintAccountInfo();
                _printAccountInfoFlag = false;
            }
        }
    }
}
