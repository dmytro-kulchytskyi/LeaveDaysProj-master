using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class Module
    {
        public virtual int Id { get; set; }
        public virtual int DefaultModuleId { get; set; }
        public virtual int LicenseId { get; set; }
        public virtual double Price { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsLocked { get; set; }
    }

    //public sealed class ModuleCsvMap:CsvClassMap<Module>
    //{
    //    public ModuleCsvMap()
    //    {
    //        Map(m => m.Id);
    //        Map(m => m.Price);
    //    }
    //}
}