using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.License
{
    public class EditLicenseModules
    {
        public string LicenseName { get; set; }
        public IList<DefaultModule> Modules { get; set; }
        public string LicenseCode { get; set; }
    }
}