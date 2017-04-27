using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModel
{
    public class LicenseInfo
    {
        public int UserId { get; set; }
        public int LicenseId { get; set; }
        public string LicenceCode { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string[] AvilableModules { get; set; }
        public int UsersNum { get; set; }
        public int UsedLicenses { get; set; }
        public bool IsPaid { get; set; }
    }

    public class SearchOption
    {
        public string LicenceCode { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}