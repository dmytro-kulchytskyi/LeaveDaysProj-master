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
        public CompanyService(IUserRepository userRepository, IRoleRepository roleRepository, ICompanyRepository companyRepository, IModuleRepository moduleRepository, ILicenseRepository licenseRepository)
        {
            this.companyRepository = companyRepository;
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
            this.moduleRepository = moduleRepository;
            this.licenseRepository = licenseRepository;
        }
        private readonly ILicenseRepository licenseRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;

        public string[] GetRolesFromLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new string[0];
            line = line.Trim(',');
            var roles = line.Split(',');
            if (roles.Length == 0)
                return new string[0];
            else return roles;
        }

        public License CreateLicense(DefaultLicense defaultLicense)
        {
            var license = new License()
            {
                DefaultLicenseId = defaultLicense.Id,
                Price = defaultLicense.Price
            };
            foreach (var defaultModule in defaultLicense.Modules)
            {
                var module = new Module()
                {
                    DefaultModuleId = defaultModule.Id,
                    Price = defaultModule.Price
                };
                moduleRepository.Save(module);
                license.Modules.Add(module);
            }
            licenseRepository.Save(license);
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