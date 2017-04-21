using leavedays.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays
{
    public static class IdentityHelper
    {
        public static bool IsInRole(this AppUser user, string roleName)
        {
            foreach (var role in user.Roles)
            {
                if (role.Name.ToLower() == roleName.ToLower())
                    return true;
            }
            return false;

        }
    }
}