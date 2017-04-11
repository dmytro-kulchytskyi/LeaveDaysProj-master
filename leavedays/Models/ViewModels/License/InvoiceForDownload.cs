using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;

namespace leavedays.Models.ViewModels.License
{
    public class InvoiceForDownload
    {
        public int Id { get; set; }
        public DateTime RecieveDate { get; set; }
        public leavedays.Models.License License { get; set; }
        public Company Company { get; set; }
        public AppUser Owner { get; set; }
    }
}