using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.License
{
    public class EditLicenseSeatsModel
    {
        public string Name { get; set; }
        public string LicenseCode { get; set; }
        public string PackageName { get; set; }
        public int PricePerUser { get; set; }
        public int TotalSeatsCount { get; set; }
        public int ActiveSeatsCount { get; set; }
    }
}