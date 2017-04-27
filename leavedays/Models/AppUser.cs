using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace leavedays.Models
{
    public class AppUser : IUser<int>
    {
        public virtual int Id { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }

        public virtual int AccessFailedCount { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual DateTimeOffset LockoutEnd { get; set; }
       
        private ISet<Role> _Roles = new HashSet<Role>();
        public virtual ISet<Role> Roles
        {
            get
            {
                return _Roles;
            }
            set
            {
                _Roles = value;
            }
        }
       
        private IList<Request> _requests;
        public virtual IList<Request> Requests
        {
            get
            {
                return _requests ?? (_requests = new List<Request>());
            }
            set
            {
                _requests = value;
            }
        }
    }
}