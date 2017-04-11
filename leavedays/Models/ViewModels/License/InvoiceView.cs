using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModels.License
{
    public class InvoiceView
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public DateTime RecieveDate { get; set; }
    }
}