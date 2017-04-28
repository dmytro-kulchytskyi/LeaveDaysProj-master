using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class ModuleShortInfo
    {
        public int DefaultModuleId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool Checked { get; set; }
    }
}