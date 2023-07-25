using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADsFusion
{
    internal class User
    {
        private string _sAMAccountName1 { get; set; }
        private string _displayName1 { get; set; }
        private string _givenName1 { get; set; }
        private string _sn1 { get; set; }
        private string _mail1 { get; set; }
        private string _title1 { get; set; }
        private string _description1 { get; set; }

        private string _sAMAccountName2 { get; set; }
        private string _displayName2 { get; set; }
        private string _givenName2 { get; set; }
        private string _sn2 { get; set; }
        private string _mail2 { get; set; }
        private string _title2 { get; set; }
        private string _description2 { get; set; }

        public User(string sAMAccountName, string displayName, string givenName, string sn, string mail, string title, string description) 
        {
            _sAMAccountName1 = sAMAccountName;
            _displayName1 = displayName;
            _givenName1 = givenName;
            _sn1 = sn;
            _mail1 = mail;
            _title1 = title;
            _description1 = description;
        }

        public User(string sAMAccountName, string displayName, string givenName, string sn, string mail, string title, string description, string sAMAccountName2, string displayName2, string givenName2, string sn2, string mail2, string title2, string description2) 
        {
            _sAMAccountName1 = sAMAccountName;
            _displayName1 = displayName;
            _givenName1 = givenName;
            _sn1 = sn;
            _mail1 = mail;
            _title1 = title;
            _description1 = description;

            _sAMAccountName2 = sAMAccountName2;
            _displayName2 = displayName2;
            _givenName2 = givenName2;
            _sn2 = sn2;
            _mail2 = mail2;
            _title2 = title2;
            _description2 = description2;
        }

    }
}
