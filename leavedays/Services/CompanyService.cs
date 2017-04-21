using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace leavedays.Services
{
    public class CompanyService
    {
        public CompanyService(IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            ICompanyRepository companyRepository, 
            ILicenseRepository licenseRepository, 
            IModuleRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
        }
        private readonly ILicenseRepository licenseRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;

       

        public License CreateLicense(DefaultLicense defaultLicense)
        {
            var license = new License()
            {
                DefaultLicenseId = defaultLicense.Id,
                Price = defaultLicense.Price,
                LicenseCode = Guid.NewGuid().ToString(),
                Seats = 1
            };
            licenseRepository.Save(license);

            var modules = defaultLicense.DefaultModules.Select(defaultModule => new Module()
            {
                DefaultModuleId = defaultModule.Id,
                Price = defaultModule.Price,
                IsActive = true,
                LicenseId = license.Id
            });
            moduleRepository.Save(modules);

            return license;
        }

        public IEnumerable<Role> GetRolesList(IEnumerable<string> roles)
        {
            var allRoles = roleRepository.GetAll();
            var userRoles = allRoles.Where(x => roles.Contains(x.Name));
            return userRoles;
        }

        public bool ContainsRole(IEnumerable<Role> roles, string roleName)
        {
            if (roles == null) return false;
            foreach (var role in roles)
                if (role.Name == roleName)
                    return true;
            return false;
        }

        public AppUser GetUserByName(string name)
        {
            return userRepository.GetByUserName(name);
        }
        public AppUser GetUserById(int id)
        {
            return userRepository.GetById(id);
        }

        public int SaveCompany(Company company)
        {
            return companyRepository.Save(company);
        }

        public IList<Company> GetCompanysByCompanyIds(IList<int> companyIds)
        {
            return companyRepository.GetByCompanyIds(companyIds);
        }

    }
}