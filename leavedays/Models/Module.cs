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
        public virtual double Price { get; set; }
        public virtual bool IsActive { get; set; }
    }
}