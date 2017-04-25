using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.License
{
    public class ModuleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public IEnumerable<DefaultLicense> Licenses { get; set; }
        public bool isAvailable { get; set; }
        public bool isLocked { get; set; }
    }
}