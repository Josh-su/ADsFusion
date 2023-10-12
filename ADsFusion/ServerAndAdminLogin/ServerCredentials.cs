using ADsFusion.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ADsFusion
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ServerCredentials : Form
    {
        private CustomName _customNames;

        private readonly int _initialHeight;

        private readonly List<TextBox> _allTextBoxesGroup1 = new List<TextBox>();

        private List<string> _groups1TextboxValues;

        private readonly StringBuilder dynamicTextboxesData = new StringBuilder();

        private int _index;

        public bool Modifying;

        /// <summary>
        /// 
        /// </summary>
        public ServerCredentials()
        {
            InitializeComponent();

            _customNames = new CustomName();

            _initialHeight = this.Height;

            _groups1TextboxValues = new List<string>();

            _allTextBoxesGroup1.Add(txtbGroups);

            Modifying = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemText"></param>
        public void InitializeCredential(string itemText)
        {
            // Call the method with the appropriate index based on the selected item
            if (itemText == $"{Credentials.Default.Domain1}, {Credentials.Default.Username1}")
            {
                LoadCredentials(1);
            }
            else if (itemText == $"{Credentials.Default.Domain2}, {Credentials.Default.Username2}")
            {
                LoadCredentials(2);
            }
            else if (itemText == $"{Credentials.Default.Domain3}, {Credentials.Default.Username3}")
            {
                LoadCredentials(3);
            }
            else if (itemText == $"{Credentials.Default.Domain4}, {Credentials.Default.Username4}")
            {
                LoadCredentials(4);
            }
            else if (itemText == $"{Credentials.Default.Domain5}, {Credentials.Default.Username5}")
            {
                LoadCredentials(5);
            }
        }

        private void LoadCredentials(int index)
        {
            _index = index;

            _customNames.Initialize(_index);

            string domain = Credentials.Default[$"Domain{index}"].ToString();
            string username = Credentials.Default[$"Username{index}"].ToString();
            string password = Credentials.Default[$"Password{index}"].ToString();
            string group = Credentials.Default[$"GroupAdmin{index}"].ToString();
            string groupsData = Credentials.Default[$"Groups{index}"].ToString();

            txtbDomain.Text = domain;
            txtbUsername.Text = username;
            txtbPassword.Text = password;
            txtbGroup.Text = group;

            _groups1TextboxValues = groupsData.Split('|').ToList();
            _groups1TextboxValues.Remove(_groups1TextboxValues.Last());

            // Load dynamic textbox data
            foreach (string value in _groups1TextboxValues)
            {
                if (_allTextBoxesGroup1.Count > 1)
                {
                    TextBox newTextbox = new TextBox
                    {
                        Size = txtbGroups.Size,
                        Location = new Point(txtbGroups.Left, _allTextBoxesGroup1[_allTextBoxesGroup1.Count - 1].Bottom + 10),
                        Text = value
                    };
                    _allTextBoxesGroup1.Add(newTextbox);
                    Controls.Add(newTextbox);
                    AdjustFormLayout();
                }
                else
                {
                    txtbGroups.Text = value;
                }
            }
        }

        private void SaveModifiedCredentials(string domain, string username, string password, string adminGroup)
        {
            Credentials.Default[$"Domain{_index}"] = domain;
            Credentials.Default[$"Username{_index}"] = username;
            Credentials.Default[$"Password{_index}"] = password;
            Credentials.Default[$"GroupAdmin{_index}"] = adminGroup;

            // Save dynamic textbox data
            StringBuilder dynamicTextboxesData = new StringBuilder();
            foreach (TextBox textBox in _allTextBoxesGroup1)
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    dynamicTextboxesData.Append(textBox.Text);
                    dynamicTextboxesData.Append("|"); // Use a separator between values
                }
            }
            Credentials.Default[$"Groups{_index}"] = dynamicTextboxesData.ToString();

            Credentials.Default.Save(); // Save changes to settings
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private bool GroupExists(string domain, string groupName)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, IdentityType.Name, groupName);

                    if (group != null)
                    {
                        // Le groupe existe, vous pouvez continuer
                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"Le groupe {groupName} n'existe pas dans le domaine {domain}.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="adminGroup"></param>
        /// <returns></returns>
        private bool LoginDomain(string domain, string username, string password, string adminGroup)
        {
            if (!GroupExists(domain, adminGroup))
            {
                return false;
            }

            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain))
                {
                    bool isAuthenticated = context.ValidateCredentials(username, password);

                    if (isAuthenticated)
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);

                        if (user.IsMemberOf(context, IdentityType.Name, adminGroup))
                        {
                            foreach (TextBox textBox in _allTextBoxesGroup1)
                            {
                                if (!string.IsNullOrEmpty(textBox.Text))
                                {
                                    // Vérifiez si le groupe spécifié dans la zone de texte existe
                                    string groupToCheck = textBox.Text;
                                    bool groupExists = GroupExists(domain, groupToCheck);

                                    if (!groupExists)
                                    {
                                        MessageBox.Show($"Le groupe {groupToCheck} n'existe pas dans le domaine {domain}.");
                                        return false;
                                    }
                                }
                            }

                            // Tous les groupes spécifiés existent
                            MessageBox.Show($"Connexion réussie pour le domaine {domain} en tant que membre de {adminGroup} !");
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Vous n'êtes pas autorisé à vous connecter à cette application.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Nom d'utilisateur ou mot de passe incorrect pour le domaine {domain}.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite : " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Update the credentials with the provided informations
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="adminGroup"></param>
        private void SaveCredentials(string domain, string username, string password, string adminGroup)
        {
            int maxSettings = 5; // Define the maximum number of sets of credentials

            for (int i = 1; i <= maxSettings; i++)
            {
                if (string.IsNullOrEmpty((string)Credentials.Default[$"Domain{i}"]))
                {
                    Credentials.Default[$"Domain{i}"] = domain;
                    Credentials.Default[$"Username{i}"] = username;
                    Credentials.Default[$"Password{i}"] = password;
                    Credentials.Default[$"GroupAdmin{i}"] = adminGroup;

                    // Save dynamic textbox information
                    dynamicTextboxesData.Clear();
                    foreach (TextBox textBox in _allTextBoxesGroup1)
                    {
                        if (!string.IsNullOrEmpty(textBox.Text))
                        {
                            dynamicTextboxesData.Append(textBox.Text);
                            dynamicTextboxesData.Append("|"); // Use a separator between values
                        }
                    }
                    Credentials.Default[$"Groups{i}"] = dynamicTextboxesData.ToString();

                    break; // Exit the loop after saving the first empty slot
                }
            }

            // Save the changes
            Credentials.Default.Save();
        }

        /// <summary>
        /// Clear all textboxes on the form and its nested containers
        /// </summary>
        private void ClearAllTextBoxes()
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox textBox)
                {
                    textBox.Clear();
                }
            }

            for (int i = _allTextBoxesGroup1.Count - 1; i >= 0; i--)
            {
                if (_allTextBoxesGroup1[i] != txtbGroups)
                {
                    Controls.Remove(_allTextBoxesGroup1[i]);
                    _allTextBoxesGroup1.RemoveAt(i);
                }
            }

            AdjustFormLayout();
        }

        #region Add & toggle OU textbox
        private void Button1_Click(object sender, EventArgs e)
        {
            if (_allTextBoxesGroup1.Count < 5)
            {
                TextBox newTextBox = new TextBox
                {
                    Size = txtbGroups.Size // Set the size
                };
                if (_allTextBoxesGroup1.Count > 0)
                {
                    newTextBox.Location = new Point(txtbGroups.Left, _allTextBoxesGroup1[_allTextBoxesGroup1.Count - 1].Bottom + 10);
                }
                else
                {
                    newTextBox.Location = new Point(txtbGroups.Left, txtbGroups.Bottom + 10);
                }
                _allTextBoxesGroup1.Add(newTextBox);
                Controls.Add(newTextBox);

                AdjustFormLayout();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (_allTextBoxesGroup1.Count > 1)
            {
                Controls.Remove(_allTextBoxesGroup1[_allTextBoxesGroup1.Count - 1]);
                _allTextBoxesGroup1.RemoveAt(_allTextBoxesGroup1.Count - 1);

                AdjustFormLayout();
            }
        }

        private void AdjustFormLayout()
        {
            // Adjust the form's height to accommodate added textboxes
            this.Height = _initialHeight + (_allTextBoxesGroup1.Count - 1) * 30;
        }
        #endregion

        private void TestConnection_Click(object sender, EventArgs e)
        {
            List<string> server1Informations = new List<string>
            {
                txtbDomain.Text,
                txtbUsername.Text,
                txtbPassword.Text,
                txtbGroup.Text
            };

            int server1NotEmptyInformations = 0;

            foreach (string information1 in server1Informations)
            {
                if (!string.IsNullOrEmpty(information1))
                {
                    server1NotEmptyInformations++;
                }
            }
            bool group1TextBoxFilled = IsAtLeastOneTextBoxFilled(_allTextBoxesGroup1);

            if (server1NotEmptyInformations > 0 && server1NotEmptyInformations < server1Informations.Count && !group1TextBoxFilled)
            {
                MessageBox.Show("Veuillez écrire toutes les informations");
            }
            else if (server1NotEmptyInformations == server1Informations.Count && group1TextBoxFilled)
            {
                LoginDomain(txtbDomain.Text, txtbUsername.Text, txtbPassword.Text, txtbGroup.Text);
            }
        }

        private void Connection_Click(object sender, EventArgs e)
        {
            List<string> server1Informations = new List<string>
            {
                txtbDomain.Text,
                txtbUsername.Text,
                txtbPassword.Text,
                txtbGroup.Text
            };

            int server1NotEmptyInformations = 0;

            bool LoginSuccess = false;

            foreach (string information1 in server1Informations)
            {
                if (!string.IsNullOrEmpty(information1))
                {
                    server1NotEmptyInformations++;
                }
            }
            bool group1TextBoxFilled = IsAtLeastOneTextBoxFilled(_allTextBoxesGroup1);

            if (server1NotEmptyInformations > 0 && server1NotEmptyInformations < server1Informations.Count && !group1TextBoxFilled)
            {
                MessageBox.Show("Veuillez écrire toute les informations pour le serveur 1");
            }
            else if (server1NotEmptyInformations == server1Informations.Count && group1TextBoxFilled)
            {
                int maxSettings = 5; // Define the maximum number of sets of credentials
                bool domainExists = false;
                // Check if there's an existing entry with the same domain
                for (int i = 1; i <= maxSettings; i++)
                {
                    if (Credentials.Default[$"Domain{i}"].ToString() == txtbDomain.Text)
                    {
                        domainExists = true;
                        break; // Exit the loop if a matching domain is found
                    }
                }
                if (domainExists)
                {
                    // Display a message to the user
                    MessageBox.Show("Des identifiants avec le même domaine existent déjà. Veuillez modifier l'entrée existante ou choisir un domaine différent.");
                }
                else // If no matching domain is found, save the new credentials
                {
                    LoginSuccess = LoginDomain(txtbDomain.Text, txtbUsername.Text, txtbPassword.Text, txtbGroup.Text);
                }
            }

            // Check if at least one domain's login is successful before saving credentials
            if (LoginSuccess)
            {
                if (Modifying) SaveModifiedCredentials(txtbDomain.Text, txtbUsername.Text, txtbPassword.Text, txtbGroup.Text);
                else SaveCredentials(txtbDomain.Text, txtbUsername.Text, txtbPassword.Text, txtbGroup.Text);

                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBoxes"></param>
        /// <returns></returns>
        private bool IsAtLeastOneTextBoxFilled(List<TextBox> textBoxes)
        {
            foreach (TextBox textBox in textBoxes)
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    return true; // Found a filled textbox, no need to check further
                }
            }
            return false; // No filled textbox found in the list
        }

        private void ServerCredentials_FormClosing(object sender, FormClosingEventArgs e)
        {
            _customNames.Close();
            Modifying = false;
            ClearAllTextBoxes();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            _customNames.ShowDialog();
        }
    }
}
