using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class Company
    {
        public virtual int Id { get; set; }
        public virtual string FullName { get; set; }
        public virtual string UrlName { get; set; }
        public virtual DateTime LastPay { get; set; }
        public virtual double Debt { get; set; }
        public virtual bool IsPaid { get; set; }
        public virtual IList<Invoice> Invoices { get; set; }
        public virtual int LicenseId { get; set; }

    }
}