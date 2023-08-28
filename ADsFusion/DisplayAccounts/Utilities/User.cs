using System.Collections.Generic;

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

        public User(
            string sAMAccountName1, string displayName1, string givenName1, string sn1, string mail1, string title1, string description1, List<string> userGroups1,
            string sAMAccountName2, string displayName2, string givenName2, string sn2, string mail2, string title2, string description2, List<string> userGroups2)
        {
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
        }
    }
}
