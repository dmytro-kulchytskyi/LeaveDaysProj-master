using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.License
{
    public class EditModuleModel
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public string Description { get; set; }
        public double Price { get; set; }
        public List<DefaultLicense> ModuleLicenses { get; set; }
        public List<DefaultLicense> AllLicenses { get; set; }
   }
}