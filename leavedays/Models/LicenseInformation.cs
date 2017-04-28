using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class LicenseInformation
    {
        public License License { get; set; }
        public Company Company { get; set; }
        public string LicenseName { get; set; }
        public IList<DefaultModule> DefaultModules { get; set; }
        public IList<ModuleShortInfo> ModulesInfoList { get; set; }
        public IList<Module> Modules { get; set; }
        public string LicenseCode { get; set; }
        public int LicensesCount { get; set; }
        public int ActiveLicensesCount { get; set; }
        public double Price { get; set; }
    }
}