﻿using System;
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
using System.Threading;
using System.DirectoryServices.ActiveDirectory;

namespace ADsFusion
{
    public partial class DisplayAccounts : Form
    {
        private readonly Settings _settings;
        private readonly ServersList _servers;
        private readonly FilterForm _filterForm;
        private readonly MergeSettings _mergeSettings;

        // Define dictionarys to store instances of AccountDetails forms.
        private readonly Dictionary<User, SingleAccountDetails> _singleAccountsDetailsForms;

        private readonly string _repositoryUserListsFilesPath;
        private readonly string _repositoryGroupsListsListsPath;

        private string _domain1;
        private string _domain2;
        private string _domain3;
        private string _domain4;
        private string _domain5;

        private List<string> _groupList1;
        private List<string> _groupList2;
        private List<string> _groupList3;
        private List<string> _groupList4;
        private List<string> _groupList5;
        private List<string> _allGroupsList;

        private List<User> _userList1;
        private List<User> _userList2;
        private List<User> _userList3;
        private List<User> _userList4;
        private List<User> _userList5;
        private readonly List<User> _actualUserList;
        private List<User> _filteredUserList;

        private readonly string _userList1Path;
        private readonly string _userList2Path;
        private readonly string _userList3Path;
        private readonly string _userList4Path;
        private readonly string _userList5Path;
        private readonly string _groupListPath;

        private bool _isBackgroundProcessRunning = false;

        // Using the word "Flag" in the variable name is a common convention to indicate that it's a boolean flag used for controlling a specific behavior
        private bool _printAccountInfoFlag = false;
        private bool _openDetailsFormFlag = false;
        private bool _selectAllItemsFlag = false;


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
            _userList3 = new List<User>();
            _userList4 = new List<User>();
            _userList5 = new List<User>();
            _actualUserList = new List<User>();
            _filteredUserList = new List<User>();

            _groupList1 = new List<string>();
            _groupList2 = new List<string>();
            _groupList3 = new List<string>();
            _groupList4 = new List<string>();
            _groupList5 = new List<string>();
            _allGroupsList = new List<string>();

            _settings = new Settings();
            _servers = new ServersList();
            _filterForm = new FilterForm();
            _filterForm.CheckedItemChanged += (s, args) => 
            {
                UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
            };
            _filterForm.RadioButtonsCheckedChanged += (s, args) =>
            {
                if(_filterForm.SelectedGroups.Count >= 1) UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
            };
            _mergeSettings = new MergeSettings();

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
            _userList3Path = Path.Combine(_repositoryUserListsFilesPath, "UserList3.json");
            _userList4Path = Path.Combine(_repositoryUserListsFilesPath, "UserList4.json");
            _userList5Path = Path.Combine(_repositoryUserListsFilesPath, "UserList5.json");
            _groupListPath = Path.Combine(_repositoryGroupsListsListsPath, "GroupList.json");

            // Define the list of files with their paths and default content
            List<(string filePath, string defaultContent)> files = new List<(string filePath, string defaultContent)>
            {
                (_userList1Path, ""),
                (_userList2Path, ""),
                (_userList3Path, ""),
                (_userList4Path, ""),
                (_userList5Path, ""),
                (_groupListPath, ""),
                // Add more files here if needed
            };

            // Check and create each file if it doesn't exist
            foreach (var (filePath, defaultContent) in files)
            {
                CheckAndCreateFile(filePath, defaultContent);
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
            if (File.Exists(filePath))
            {
                // File exists, delete it
                File.Delete(filePath);
            }

            // Create the file with default content
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.Write(defaultContent);
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
        /// <param name="ints"></param>
        private void SetUserListFromJson(List<int> ints)
        {
            if (ints.Count > 0)
            {
                if (File.Exists(_userList1Path))
                {
                    _userList1 = JsonManager.ReadFromJson<User>(_userList1Path);
                    _userList1 = _userList1.Distinct().ToList();
                }
                else
                {
                    // The file doesn't exist or is empty, create an empty list
                    _userList1 = new List<User>();
                }
                if (File.Exists(_userList2Path))
                {
                    _userList2 = JsonManager.ReadFromJson<User>(_userList2Path);
                    _userList2 = _userList2.Distinct().ToList();
                }
                else
                {
                    // The file doesn't exist or is empty, create an empty list
                    _userList2 = new List<User>();
                }
                if (File.Exists(_userList3Path))
                {
                    _userList3 = JsonManager.ReadFromJson<User>(_userList3Path);
                    _userList3 = _userList3.Distinct().ToList();
                }
                else
                {
                    // The file doesn't exist or is empty, create an empty list
                    _userList3 = new List<User>();
                }
                if (File.Exists(_userList4Path))
                {
                    _userList4 = JsonManager.ReadFromJson<User>(_userList3Path);
                    _userList4 = _userList4.Distinct().ToList();
                }
                else
                {
                    // The file doesn't exist or is empty, create an empty list
                    _userList4 = new List<User>();
                }
                if (File.Exists(_userList5Path))
                {
                    _userList5 = JsonManager.ReadFromJson<User>(_userList3Path);
                    _userList5 = _userList5.Distinct().ToList();
                }
                else
                {
                    // The file doesn't exist or is empty, create an empty list
                    _userList5 = new List<User>();
                }
                UpdateActualUserList(ints);
                UpdateGroupsListAndSaveToJson(_actualUserList, _groupListPath);
                _allGroupsList = JsonManager.ReadFromJson<string>(_groupListPath);
                _filterForm.ListGroups.Clear();
                _filterForm.ListGroups = _allGroupsList;
                _filterForm.UpdateGroups();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<int> CheckIfLogged()
        {
            List<int> ints = new List<int>();

            for (int i = 1; i <= 5; i++) // Assuming you have 5 settings
            {
                string domain = Properties.Credentials.Default[$"Domain{i}"].ToString();

                if (!string.IsNullOrEmpty(domain))
                {
                    ints.Add(i);
                }
            }
            UpdateLastUpdateTime(_groupListPath);
            return ints;
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
            ClearItemFromListBox();

            string searchText = textBox1.Text.Normalize().Trim().ToLower(); // Convert to lowercase once

            foreach (User user in _filteredUserList)
            {
                string displayText = $"{user.Domain ?? "n/a"} || {user.SAMAccountName ?? "n/a"}, {user.DisplayName ?? "n/a"}";

                if ((searchText.Length >= 3 || searchText.Length == 0) && displayText.Normalize().Trim().ToLower().Contains(searchText))
                {
                    AddItemToListBox(displayText);
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
            _servers.ShowDialog();
        }

        /// <summary>
        /// impressions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {

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
                        List<string> allUserGroups = (user.UserGroups ?? new List<string>());

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

                        if (user.UserGroups?.Intersect(groups).Any() ?? false)
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
        /// <param name="ints"></param>
        /// <returns></returns>
        private async Task UpdateAllAsync(List<int> ints)
        {
            _domain1 = Properties.Credentials.Default.Domain1;
            _domain2 = Properties.Credentials.Default.Domain2;
            _domain3 = Properties.Credentials.Default.Domain3;
            _domain4 = Properties.Credentials.Default.Domain4;
            _domain5 = Properties.Credentials.Default.Domain5;

            _groupList1 = Properties.Credentials.Default.Groups1.Split('|').ToList();
            _groupList1.RemoveAll(group => string.IsNullOrWhiteSpace(group)); // remove all the empty entry
            _groupList2 = Properties.Credentials.Default.Groups2.Split('|').ToList();
            _groupList2.RemoveAll(group => string.IsNullOrWhiteSpace(group)); // remove all the empty entry
            _groupList3 = Properties.Credentials.Default.Groups3.Split('|').ToList();
            _groupList3.RemoveAll(group => string.IsNullOrWhiteSpace(group)); // remove all the empty entry
            _groupList4 = Properties.Credentials.Default.Groups4.Split('|').ToList();
            _groupList4.RemoveAll(group => string.IsNullOrWhiteSpace(group)); // remove all the empty entry
            _groupList5 = Properties.Credentials.Default.Groups5.Split('|').ToList();
            _groupList5.RemoveAll(group => string.IsNullOrWhiteSpace(group)); // remove all the empty entry

            if (ints.Count == 0)
            {
                _servers.ShowDialog();
                await UpdateAllAsync(CheckIfLogged());
            }
            else if (ints.Count >= 1)
            {
                progressBar1.Visible = true;
                foreach (int i in ints)
                {
                    if (i == 1) _userList1 = await Task.Run(() => UpdateUserListAsync(_userList1Path, _domain1, _groupList1));
                    if (i == 2) _userList2 = await Task.Run(() => UpdateUserListAsync(_userList2Path, _domain2, _groupList2));
                    if (i == 3) _userList3 = await Task.Run(() => UpdateUserListAsync(_userList3Path, _domain3, _groupList3));
                    if (i == 4) _userList4 = await Task.Run(() => UpdateUserListAsync(_userList4Path, _domain4, _groupList4));
                    if (i == 5) _userList5 = await Task.Run(() => UpdateUserListAsync(_userList5Path, _domain5, _groupList5));
                }
            }
            if (ints.Count != 0)
            {
                progressBar1.Visible = false;
                SetUserListFromJson(ints);
                UpdateFilteredUserList(_filterForm.SelectedGroups, _filterForm.SelectAllMatchingGroups);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userListPath"></param>
        /// <param name="domain"></param>
        /// <param name="groupList"></param>
        /// <returns></returns>
        private async Task<List<User>> UpdateUserListAsync(string userListPath, string domain, List<string> groupList)
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
                    GetADUsers(groupName, ActiveUsersAD, userListPath, domain);
                });

                // Wait for all the tasks to complete before reading from the JSON file.
                await Task.WhenAll(_userTasks);

                // Read the data back from the JSON file into ActiveUsersAD.
                List<User> userListToReturn = JsonManager.ReadFromJson<User>(userListPath);
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
        /// <param name="ActiveUsersAD"></param>
        /// <param name="userListPath"></param>
        /// <param name="domain"></param>
        private void GetADUsers(string groupName, List<User> ActiveUsersAD, string userListPath, string domain)
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

                                    User userToAdd = new User(
                                        domain: domain,
                                        sAMAccountName: Convert.ToString(userPrincipal.SamAccountName),
                                        displayName: Convert.ToString(userPrincipal.DisplayName),
                                        givenName: Convert.ToString(userPrincipal.GivenName),
                                        sn: Convert.ToString(userPrincipal.Surname),
                                        mail: Convert.ToString(userPrincipal.EmailAddress),
                                        title: Convert.ToString(de.Properties["extensionAttribute2"].Value?.ToString()),
                                        description: Convert.ToString(userPrincipal.Description),
                                        userGroups: groups);

                                    // Add the user to the list of active users within a lock
                                    lock (activeUsersLock)
                                    {
                                        ActiveUsersAD.Add(userToAdd);

                                        // Check if the batch size is reached or if we processed all users.
                                        if (ActiveUsersAD.Count % batchSize == 0 || progressCounter == totalActiveUserPrincipal - 1)
                                        {
                                            // Save the current batch to the JSON file.
                                            JsonManager.SaveToJson(ActiveUsersAD, userListPath, false);

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
                if (user.UserGroups != null)
                {
                    foreach (string group in user.UserGroups)
                    {
                        _allGroupsList.Add(group);
                    }
                }
            }

            // Remove duplicate group names by applying Distinct() and updating _groupList.
            _allGroupsList = _allGroupsList.Distinct().ToList();

            // Save the list of group names to a JSON file.
            JsonManager.SaveToJson(_allGroupsList, groupListPath, true);
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
            string websiteUrl = "https://josh-su.github.io/ADsFusion/";

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
                
                selectedUser = _actualUserList.FirstOrDefault(user =>
                {
                    string userDisplayText = $"{user.Domain ?? "n/a"} || {user.SAMAccountName ?? "n/a"}, {user.DisplayName ?? "n/a"}";
                    //string userDisplayText = $"{user.SAMAccountName1 ?? user.SAMAccountName2 ?? user.SAMAccountName3 ?? user.SAMAccountName4 ?? user.SAMAccountName5}, {user.DisplayName1 ?? user.DisplayName2 ?? user.DisplayName3 ?? user.DisplayName4 ?? user.DisplayName5}";
                    return userDisplayText.ToLower() == displayText.ToLower();
                });

                if (selectedUser != null)
                {
                    if (!_singleAccountsDetailsForms.ContainsKey(selectedUser))
                    {
                        SingleAccountDetails newForm = new SingleAccountDetails();
                        newForm.InitializeWithUser(selectedUser); // Pass the selected user to the form

                        _singleAccountsDetailsForms.Add(selectedUser, newForm);
                        newForm.FormClosed += (s, args) => _singleAccountsDetailsForms.Remove(selectedUser);
                    }

                    // Show the form, whether it's a new instance or an existing one.
                    if (_singleAccountsDetailsForms.ContainsKey(selectedUser))
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
        private void UpdateActualUserList(List<int> ints)
        {
            _actualUserList.Clear();
            foreach (int i in ints)
            {
                switch (i)
                {
                    case 1:
                        _actualUserList.AddRange(_userList1);
                        break;
                    case 2:
                        _actualUserList.AddRange(_userList2);
                        break;
                    case 3:
                        _actualUserList.AddRange(_userList3);
                        break;
                    case 4:
                        _actualUserList.AddRange(_userList4);
                        break;
                    case 5:
                        _actualUserList.AddRange(_userList5);
                        break;
                    default:
                        // Handle the case where 'i' is out of range.
                        break;
                }
            }
        }

        /// <summary>
        /// open the Settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button10_Click(object sender, EventArgs e)
        {
            _settings.Show();
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
        private void ClearItemFromListBox()
        {
            listBox1.Items.Clear(); // Remove an item from the ListBox
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

                selectedUser = _actualUserList.FirstOrDefault(user =>
                {
                    string userDisplayText = $"{user.Domain ?? "n/a"} || {user.SAMAccountName ?? "n/a"}, {user.DisplayName ?? "n/a"}";
                    return userDisplayText.ToLower() == displayText.ToLower();
                });

                if (selectedUser != null)
                {
                    //print logic...
                    MessageBox.Show(selectedUser.SAMAccountName + selectedUser.SAMAccountName);


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

                selectedUser = _actualUserList.FirstOrDefault(user =>
                {
                    string userDisplayText = $"{user.Domain ?? "n/a"} || {user.SAMAccountName ?? "n/a"}, {user.DisplayName ?? "n/a"}";
                    return userDisplayText.ToLower() == displayText.ToLower();
                });

                if (selectedUser != null)
                {
                    MessageBox.Show(selectedUser.SAMAccountName + " " + selectedUser.SAMAccountName);
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
                        foreach(int index in listBox1.SelectedIndices)
                        {
                            listBox1.SetSelected(index, false);
                        }
                        listBox1.SetSelected(0, true);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }

                // Handle Ctrl+Down: Select the last item
                if (e.KeyCode == Keys.Down)
                {
                    int lastIndex = listBox1.Items.Count - 1;
                    if (listBox1.Items.Count > 0)
                    {
                        foreach (int index in listBox1.SelectedIndices)
                        {
                            listBox1.SetSelected(index, false);
                        }
                        listBox1.SetSelected(lastIndex, true);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }

                // Handle Ctrl+A for selecting all items
                if (e.KeyCode == Keys.A)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    _selectAllItemsFlag = true;
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

            // Handle Enter key
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                _openDetailsFormFlag = true;
                return;
            }

            // Check if the pressed key is Escape (Esc)
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                this.Close();
            }
        }

        private void DisplayAccounts_KeyUp(object sender, KeyEventArgs e)
        {
            if (_printAccountInfoFlag) PrintAccountInfo(); _printAccountInfoFlag = false;
            if (_openDetailsFormFlag) OpenDetailsForm(); _openDetailsFormFlag = false;
            if (_selectAllItemsFlag)
            {
                // Select all items in the ListBox
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    listBox1.SetSelected(i, true);
                }
                _selectAllItemsFlag = false;
            }
        }

        /// <summary>
        /// Check if there a background process before closing the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayAccounts_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Display a confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure you want to close the form?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check the user's response
            if (result == DialogResult.No)
            {
                // Cancel the form closing event to prevent the application from closing
                e.Cancel = true;
            }

            // Check if the background process is running
            if (_isBackgroundProcessRunning)
            {
                // Display a message or take appropriate action to inform the user
                MessageBox.Show("Please wait for the background process to finish \nbefore closing the application.");
                // Cancel the form closing event to prevent the application from closing
                e.Cancel = true;
            }
        }
    }
}
