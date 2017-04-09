using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace leavedays.Models.Identity
{
    public class LeavedaysIdentity : IPrincipal
    {
        private AppUser user;
        private IIdentity identity;
        public LeavedaysIdentity(AppUser user, IIdentity identity)
        {
            this.user = user;
            this.identity = identity;
        }

        public IIdentity Identity
        {
            get
            {
                return identity;
            }
        }

        public bool IsInRole(string role)
        {
            if (user.Roles == null) return false;
            foreach (var r in user.Roles)
                if (r.Name == role)
                    return true;
            return false;
        }
    }
}