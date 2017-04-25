using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class ModuleChange
    {
        public virtual int Id { get; set; }
        public virtual int ModuleId { get; set; }
        public virtual bool IsLocked { get; set; }
        public virtual double Price { get; set; }
        public virtual DateTime StartDate { get; set; }
    }
}