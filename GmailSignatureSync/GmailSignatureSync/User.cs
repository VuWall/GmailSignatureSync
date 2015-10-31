using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailSignatureSync
{
    public class User
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string CellNumber { get; set; }
        public string SkypeName { get; set; }

        public User(string accountName, string displayName, string jobTitle, string phoneNumber, string cellNumber, string skypeName)
        {
            AccountName = accountName;
            DisplayName = displayName;
            JobTitle = jobTitle;
            PhoneNumber = phoneNumber;
            CellNumber = cellNumber;
            SkypeName = skypeName;
        }
    }
}
