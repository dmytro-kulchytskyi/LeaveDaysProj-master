using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.ViewModel
{
    public class ViewRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string VacationInterval { get; set; }
        public string RequestBase { get; set; }
        public DateTime SigningDate { get; set; }
        public RequestStatus IsAccepted { get; set; }
    }
}