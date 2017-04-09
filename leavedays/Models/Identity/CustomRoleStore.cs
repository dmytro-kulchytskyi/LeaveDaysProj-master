using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;
using System.Threading.Tasks;
using leavedays.Models.Repository.Interfaces;

namespace leavedays.Models.Identity
{
    public class CustomRoleStore : IRoleStore<Role, int>
    {
        private readonly IRoleRepository siteGroupRepository;
        public CustomRoleStore(IRoleRepository siteGroupRepository)
        {
            this.siteGroupRepository = siteGroupRepository;
        }
        
        public Task CreateAsync(Role role)
        {
            return Task.FromResult(siteGroupRepository.Save(role));
        }

        public Task DeleteAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<Role> FindByIdAsync(int roleId)
        {
            return Task.FromResult(siteGroupRepository.GetById(roleId));
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            return Task.FromResult(siteGroupRepository.GetByName(roleName));
        }

        public Task UpdateAsync(Role role)
        {
            return Task.FromResult(siteGroupRepository.Save(role));
        }
    }
}