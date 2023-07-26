using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ADsFusion
{
    internal class User
    {
        public string SAMAccountName1 { get; set; }
        public string DisplayName1 { get; set; }
        public string GivenName1 { get; set; }
        public string Sn1 { get; set; }
        public string Mail1 { get; set; }
        public string Title1 { get; set; }
        public string Description1 { get; set; }
        public List<string> UserGroups1 { get; set; }

        public string SAMAccountName2 { get; set; }
        public string DisplayName2 { get; set; }
        public string GivenName2 { get; set; }
        public string Sn2 { get; set; }
        public string Mail2 { get; set; }
        public string Title2 { get; set; }
        public string Description2 { get; set; }
        public List<string> UserGroups2 { get; set; }

        public User(string sAMAccountName, string displayName, string givenName, string sn, string mail, string title, string description, List<string> userGroups)
        {
            SAMAccountName1 = sAMAccountName;
            DisplayName1 = displayName;
            GivenName1 = givenName;
            Sn1 = sn;
            Mail1 = mail;
            Title1 = title;
            Description1 = description;
            UserGroups1 = userGroups;
        }

        public User(string sAMAccountName, string displayName, string givenName, string sn, string mail, string title, string description, List<string> userGroups, string sAMAccountName2, string displayName2, string givenName2, string sn2, string mail2, string title2, string description2, List<string> userGroups2)
            : this(sAMAccountName, displayName, givenName, sn, mail, title, description, userGroups)
        {
            SAMAccountName2 = sAMAccountName2;
            DisplayName2 = displayName2;
            GivenName2 = givenName2;
            Sn2 = sn2;
            Mail2 = mail2;
            Title2 = title2;
            Description2 = description2;
            UserGroups2 = userGroups2;
        }
    }

    internal class JsonManager
    {
        public static void SaveToJson(List<User> users, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonData = JsonSerializer.Serialize(users, options);
            File.WriteAllText(filePath, jsonData);
        }

        public static List<User> ReadFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<User>();

            string jsonData = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonData);
        }
    }
}
