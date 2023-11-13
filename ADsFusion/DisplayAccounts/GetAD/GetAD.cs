using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADsFusion
{
    internal class GetAD
    {
        private readonly DisplayAccounts _displayAccountsForm; // Assuming DisplayAccounts is the form class

        public GetAD(DisplayAccounts displayAccountsForm)
        {
            _displayAccountsForm = displayAccountsForm;
        }

        private readonly object activeUsersLock = new object(); // Lock object
        private readonly List<Task> _userTasks = new List<Task>();

        public async Task GetADUsersAsync(string groupName, List<User> ActiveUsersAD, string userListPath, string domain)
        {
            var progressCounter = 0;
            // Define the batch size (e.g., 50 users at a time).
            const int batchSize = 50;

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
                                        description: Convert.ToString(userPrincipal.Description)
                                        );

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

                                    // Increment progressCounter safely
                                    int currentProgress = Interlocked.Increment(ref progressCounter);

                                    // Batch progress update every, for example, 10 users
                                    if (currentProgress % 10 == 0 || currentProgress == totalActiveUserPrincipal)
                                    {
                                        // Update the progress bar on the UI thread.
                                        _displayAccountsForm.Invoke(new Action(() =>
                                        {
                                            _displayAccountsForm.progressBar1.Value = (int)((double)currentProgress / totalActiveUserPrincipal * 100);
                                        }));
                                    }
                                }));
                                totalActiveUserPrincipal++;
                            }
                        }
                    }
                }
            }

            // Wait for all the tasks to complete before reading from the JSON file.
            await Task.WhenAll(_userTasks);

            // Final update after all tasks are completed
            _displayAccountsForm.Invoke(new Action(() =>
            {
                _displayAccountsForm.progressBar1.Value = 100;
            }));
        }

        public static List<string> GetADUserGroups(string domain, string username)
        {
            List<string> groups = new List<string>();

            // Create the PrincipalContext for the domain.
            using (var context = new PrincipalContext(ContextType.Domain, domain))
            {
                // Find the user by username.
                using (var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username))
                {
                    if (userPrincipal != null)
                    {
                        // The user is active. You can now proceed to retrieve user data and create User objects.

                        // Get the user's groups.
                        var groupsMembership = userPrincipal.GetGroups();

                        foreach (var groupMembership in groupsMembership)
                        {
                            groups.Add(groupMembership.Name);
                        }

                        // Now 'groups' contains the names of groups that the user is a member of.
                    }
                    else
                    {
                        // Handle the case where the user is not found.
                        Console.WriteLine($"User with username '{username}' not found in domain '{domain}'.");
                    }
                }
            }

            return groups;
        }
    }
}
