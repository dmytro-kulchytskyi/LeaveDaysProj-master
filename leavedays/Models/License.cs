using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class License
    {
        public virtual int Id { get; set; }
        public virtual int DefaultLicenseId { get; set; }
        public virtual double Price { get; set; }
        public virtual string LicenseCode { get; set; }
        public virtual int Seats { get; set; }
        public virtual bool IsLocked { get; set; }
        public virtual bool IsPaid { get; set; }
    }
}
