using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class Invoice
    {
        public virtual int Id { get; set; }
        public virtual Company Company { get; set; }
        public virtual DateTime RecieveDate { get; set; }
        public virtual bool isDeleted { get; set; }
    }
}