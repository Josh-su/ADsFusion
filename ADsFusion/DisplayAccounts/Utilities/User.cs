using System.Collections.Generic;

namespace ADsFusion
{
    internal class User
    {
        public string Domain { get; set; }

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

        public string SAMAccountName3 { get; set; }
        public string DisplayName3 { get; set; }
        public string GivenName3 { get; set; }
        public string Sn3 { get; set; }
        public string Mail3 { get; set; }
        public string Title3 { get; set; }
        public string Description3 { get; set; }
        public List<string> UserGroups3 { get; set; }

        public string SAMAccountName4 { get; set; }
        public string DisplayName4 { get; set; }
        public string GivenName4 { get; set; }
        public string Sn4 { get; set; }
        public string Mail4 { get; set; }
        public string Title4 { get; set; }
        public string Description4 { get; set; }
        public List<string> UserGroups4 { get; set; }

        public string SAMAccountName5 { get; set; }
        public string DisplayName5 { get; set; }
        public string GivenName5 { get; set; }
        public string Sn5 { get; set; }
        public string Mail5 { get; set; }
        public string Title5 { get; set; }
        public string Description5 { get; set; }
        public List<string> UserGroups5 { get; set; }

        public User(
            string domain = null,
            string sAMAccountName1 = null, string displayName1 = null, string givenName1 = null, string sn1 = null, string mail1 = null, string title1 = null, string description1 = null, List<string> userGroups1 = null,
            string sAMAccountName2 = null, string displayName2 = null, string givenName2 = null, string sn2 = null, string mail2 = null, string title2 = null, string description2 = null, List<string> userGroups2 = null,
            string sAMAccountName3 = null, string displayName3 = null, string givenName3 = null, string sn3 = null, string mail3 = null, string title3 = null, string description3 = null, List<string> userGroups3 = null,
            string sAMAccountName4 = null, string displayName4 = null, string givenName4 = null, string sn4 = null, string mail4 = null, string title4 = null, string description4 = null, List<string> userGroups4 = null,
            string sAMAccountName5 = null, string displayName5 = null, string givenName5 = null, string sn5 = null, string mail5 = null, string title5 = null, string description5 = null, List<string> userGroups5 = null)
        {
            Domain = domain;

            SAMAccountName1 = sAMAccountName1;
            DisplayName1 = displayName1;
            GivenName1 = givenName1;
            Sn1 = sn1;
            Mail1 = mail1;
            Title1 = title1;
            Description1 = description1;
            UserGroups1 = userGroups1;

            SAMAccountName2 = sAMAccountName2;
            DisplayName2 = displayName2;
            GivenName2 = givenName2;
            Sn2 = sn2;
            Mail2 = mail2;
            Title2 = title2;
            Description2 = description2;
            UserGroups2 = userGroups2;

            SAMAccountName3 = sAMAccountName3;
            DisplayName3 = displayName3;
            GivenName3 = givenName3;
            Sn3 = sn3;
            Mail3 = mail3;
            Title3 = title3;
            Description3 = description3;
            UserGroups3 = userGroups3;

            SAMAccountName4 = sAMAccountName4;
            DisplayName4 = displayName4;
            GivenName4 = givenName4;
            Sn4 = sn4;
            Mail4 = mail4;
            Title4 = title4;
            Description4 = description4;
            UserGroups4 = userGroups4;

            SAMAccountName5 = sAMAccountName5;
            DisplayName5 = displayName5;
            GivenName5 = givenName5;
            Sn5 = sn5;
            Mail5 = mail5;
            Title5 = title5;
            Description5 = description5;
            UserGroups5 = userGroups5;
        }
    }
}
