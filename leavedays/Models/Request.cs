using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class Request
    {
        public virtual int Id { get; set; }
        public virtual AppUser User { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual string Status { get; set; }
        public virtual string RequestBase { get; set; }
        public virtual DateTime SigningDate { get; set; }
        public virtual string VacationDates { get; set; }
        public virtual RequestStatus IsAccepted { get; set; }
    }

    public enum RequestStatus
    {
        Accepted = 1,
        NotAccepted,
        InProgress
    }
}