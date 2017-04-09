using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Principal;

namespace leavedays.Models.Identity
{
    public class CustomUserManager : UserManager<AppUser, int>
    {
        public CustomUserManager(IUserStore<AppUser, int> store) : base(store)
        {
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(AppUser user, string authenticationType)
        {
            return new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Role, "customer") });
        }
    }
}