using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Services
{
    public class LicenseService
    {
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly IDefaultModuleRepository defaultModuleRepository;
        private readonly IDefaultLicenseRepository defaultLicenseRepository;

        public LicenseService(
          IUserRepository userRepository,
          ILicenseRepository licenseRepository,
          ICompanyRepository companyRepository,
          IInvoiceRepository invoiceRepository,
          IModuleRepository moduleRepository,
          IDefaultModuleRepository defaultModuleRepository,
          IDefaultLicenseRepository defaultLicenseRepository)
        {
            this.userRepository = userRepository;;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
            this.defaultLicenseRepository = defaultLicenseRepository;
            this.defaultModuleRepository = defaultModuleRepository;
        }

        public List<LicenseInfo> GetLicenseInfoList()
        {
            var result = licenseRepository.GetLicenseInformation().ToList();
            return result;
        }

        public List<LicenseInfo> GetSearchedLicenseInfo(string searchLine)
        {
            var result = licenseRepository.GetSearchedInformation(searchLine).ToList();
            return result;
        }

        public List<LicenseInfo> GetAdwenchedSearchLicenseInfo(SearchOption option)
        {
            var result = licenseRepository.GetAdwenchedSearchedInformation(option).ToList();
            return result;
        }

        public IList<DefaultModule> GetDefaultModules(License license, bool? moduleStatus = null)
        {

            var modules = moduleRepository.GetByLicenseId(license.Id, moduleStatus);

            var defaultModules = modules.Select(module =>
            {
                var m = defaultModuleRepository.GetById(module.Id);
                m.Price = module.Price;
                return m;
            }).ToList();

            return defaultModules;

        }

        public int EditModules(int userId, IEnumerable<string> moduleNames, bool moduleStatus)
        {
            var user = userRepository.GetById(userId);
            if (user == null) return 0;

            var companyId = user.CompanyId;

            var company = companyRepository.GetById(companyId);
            if (company == null) return 0;

            var licenseId = company.LicenseId;

            var modules = moduleRepository.GetByLicenseId(licenseId);
            if (modules == null) return 0;

            modules = modules.Where(module => moduleNames.Contains(defaultModuleRepository.GetById(module.DefaultModuleId).Name)).ToList();
            foreach (var m in modules)
            {
                if (m == null) return 0;

                m.IsActive = moduleStatus;
                moduleRepository.Save(m);
            }
            return 1;
        }

        public License GetLicenseByUserId(int userId)
        {
            var user = userRepository.GetById(userId);

            var company = companyRepository.GetById(user.CompanyId);
            var license = licenseRepository.GetById(company.LicenseId);

            return license;
        }

        public LicenseInformation GetLicenseInfo(int userId)
        {
            var user = userRepository.GetById(userId);

            var company = companyRepository.GetById(user.CompanyId);
            var license = licenseRepository.GetById(company.LicenseId);

            var defaultLicense = defaultLicenseRepository.GetById(license.DefaultLicenseId);

            var licenseInfo = new LicenseInformation()
            {
                Company = company,
                License = license,
                LicenseName = defaultLicense.Name,
                LicenseCode = license.LicenseCode,
                LicensesCount = license.Seats,
                ActiveLicensesCount = companyRepository.GetUsersCount(company.Id),
                Price = license.Price
            };
            return licenseInfo;
        }

        public int EditLicenseSeats(int userId, int count)
        {
            var user = userRepository.GetById(userId);
            if (user == null) return 0;

            var company = companyRepository.GetById(user.CompanyId);
            if (company == null) return 0;

            var license = licenseRepository.GetById(company.Id);
            if (license == null) return 0;

            if (count < 0)
            {
                if (license.Seats + count <= 0)
                    return 0;
            }

            license.Seats += count;
            licenseRepository.Save(license);

            return 1;
        }
    }
}