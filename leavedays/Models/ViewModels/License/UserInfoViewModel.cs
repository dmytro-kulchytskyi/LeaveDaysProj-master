using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;

namespace leavedays.Models.ViewModels.License
{
    public class UserInfoViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public leavedays.Models.License License { get; set; }
        public Company Company { get; set; }
        public IList<ModuleInfo> Modules { get; set; }
    }
}