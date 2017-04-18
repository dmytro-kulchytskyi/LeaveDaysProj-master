using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModel;
using leavedays.Models.ViewModels.License;
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
            this.userRepository = userRepository;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
            this.defaultLicenseRepository = defaultLicenseRepository;
            this.defaultModuleRepository = defaultModuleRepository;
        }

        public List<LicenseInfo> GetLicenseInfoList()
        {
            IList<Company> companys = companyRepository.GetAll();
            List<int> companyIds = companys.Select(m => m.Id).ToList();
            IList<AppUser> owners = userRepository.GetOwnersByCompanyIds(companyIds);
            IList<License> licenses = licenseRepository.GetAll();
            var result = owners.Select(m => new LicenseInfo
            {
                CompanyName = companys.Where(n => n.Id == m.CompanyId).Select(n => n.FullName).First(),
                ContactPerson = m.FirstName + " " + m.LastName,
                Email = m.UserName,
                PhoneNumber = m.PhoneNumber,
                LicenseId = companys.Where(n => n.Id == m.CompanyId).Select(n => n.LicenseId).First(),
                LicenceCode = licenses.
                    Where(n => n.Id == companys.Where(l => l.Id == m.CompanyId).
                    Select(l => l.LicenseId).First()).
                    Select(n => n.LicenseCode).First()
            }).ToList();
            return result;
        }

        public EditLicenseModules GetModules(int userId, bool? moduleStatus = null)
        {
            var user = userRepository.GetById(userId);
            var companyId = user.CompanyId;
            var company = companyRepository.GetById(companyId);
            var licenseId = company.LicenseId;
            var license = licenseRepository.GetById(licenseId);
            var disabledModules = moduleRepository.GetByLicenseId(licenseId, moduleStatus);
            var defaultModules = disabledModules.Select(module => defaultModuleRepository.GetById(module.Id)).ToList();

            var model = new EditLicenseModules()
            {
                LicenseName = defaultLicenseRepository.GetById(license.DefaultLicenseId).Name,
                LicenseCode = license.LicenseCode,
                Modules = defaultModules
            };
            return model;

        }

        public void EditModules(int userId, IEnumerable<string> moduleNames, bool moduleStatus)
        {
            var user = userRepository.GetById(userId);
            var companyId = user.CompanyId;
            var company = companyRepository.GetById(companyId);
            var licenseId = company.LicenseId;
            var modules = moduleRepository.GetByLicenseId(licenseId);

            modules = modules.Where(module => moduleNames.Contains(defaultModuleRepository.GetById(module.DefaultModuleId).Name)).ToList();
            foreach (var m in modules)
            {
                m.IsActive = moduleStatus;
                moduleRepository.Save(m);
            }
        }
    }
}