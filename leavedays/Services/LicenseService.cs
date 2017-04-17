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
        public LicenseService(
          IUserRepository userRepository,
          ILicenseRepository licenseRepository,
          ICompanyRepository companyRepository,
          IInvoiceRepository invoiceRepository,
          IModuleRepository moduleRepository)
        {
            this.userRepository = userRepository;;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
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
    }
}