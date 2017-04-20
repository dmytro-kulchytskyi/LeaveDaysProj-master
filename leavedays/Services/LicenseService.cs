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
    }
}