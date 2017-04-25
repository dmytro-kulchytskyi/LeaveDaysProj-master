using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.License
{
    public class CreateLicense
    {
        public string LicenseName { get; set; }
        public IList<DefaultModule> Modules { get; set; }
        public int LicenseId { get; set; }
    }
}