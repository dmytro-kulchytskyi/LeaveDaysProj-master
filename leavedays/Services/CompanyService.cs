﻿using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModels.Account;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace leavedays.Services
{
    public class CompanyService
    {
        public CompanyService(IUserRepository userRepository,
            IRoleRepository roleRepository,
            ICompanyRepository companyRepository,
            ILicenseRepository licenseRepository,
            IModuleRepository moduleRepository,
            LicenseService licenseService)
        {
            this.moduleRepository = moduleRepository;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
            this.licenseService = licenseService;
        }

        private readonly LicenseService licenseService;
        private readonly ILicenseRepository licenseRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;

        public bool IsCompanyDomainUniq(string domain)
        {
            return companyRepository.GetByUrlName(domain.ToLower()) == null;
        }


        public Result<Company> CreateCompany(string companyName, string domain, string licenseName)
        {
            if (!IsCompanyDomainUniq(domain))
            {
                return Result<Company>.Error("A company with this URL already exists");
            }
            var licenseResult = licenseService.CreateLicense(licenseName);
            if (!licenseResult.Succed)
            {
                return Result<Company>.Error("Error while creating new user");
            }
            var license = licenseResult.GetResult();
            
            var company = new Company()
            {
                FullName = companyName,
                UrlName = domain,
                LicenseId = license.Id
            };
            var companyId = companyRepository.Save(company);
            if (companyId == 0)
            {
                return Result<Company>.Error("Error while creating new user");
            }
            return Result<Company>.Success(company);
        }

        public async Task<Result<AppUser>> CreateUserAsync(CreateUser userInfo, UserManager<AppUser, int> userManager)
        {
            var role = roleRepository.GetByName(userInfo.Role);
            if (role == null)
            {
                return Result<AppUser>.Error("Error while creating new user");
            }
            var roles = new HashSet<Role>();
            roles.Add(role);

            var user = new AppUser()
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Email = userInfo.Email,
                UserName = userInfo.UserName,
                CompanyId = userInfo.Comapany.Id,
                PhoneNumber = userInfo.PhoneNumber,
                Password = userInfo.Password,
                Roles = roles
            };

            var result = await userManager.CreateAsync(user, user.Password);
            if (!result.Succeeded)
            {
                return Result<AppUser>.Error(result.Errors.First());
            }

            return Result<AppUser>.Success(user);
        }

        public IEnumerable<Role> GetRolesList(IEnumerable<string> roles)
        {
            var userRoles = roleRepository.GetByName(roles);

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

        public Company GetById(int id)
        {
            return companyRepository.GetById(id);
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