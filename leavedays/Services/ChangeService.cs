using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Services
{
    public class ChangeService
    {
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly IDefaultModuleRepository defaultModuleRepository;
        private readonly IDefaultLicenseRepository defaultLicenseRepository;
        private readonly IModuleChangeRepository moduleChangeRepository;

        public ChangeService(
          IUserRepository userRepository,
          ILicenseRepository licenseRepository,
          ICompanyRepository companyRepository,
          IInvoiceRepository invoiceRepository,
          IModuleRepository moduleRepository,
          IDefaultModuleRepository defaultModuleRepository,
          IDefaultLicenseRepository defaultLicenseRepository,
          IModuleChangeRepository moduleChangeRepository)
        {
            this.userRepository = userRepository; ;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
            this.defaultLicenseRepository = defaultLicenseRepository;
            this.defaultModuleRepository = defaultModuleRepository;
            this.moduleChangeRepository = moduleChangeRepository;
        }

        public void ApplyChanges()
        {
            var currentDSate = DateTime.Now;
            var moduleNeedToChange = moduleChangeRepository.GetByDate(currentDSate.Year, currentDSate.Month, currentDSate.Day);
            List<Module> editedModule = new List<Module>(); 
            foreach (var editModule in moduleNeedToChange)
            {
                Module module = moduleRepository.GetById(editModule.ModuleId);
                module.IsLocked = editModule.IsLocked;
                module.Price = editModule.Price;
                editedModule.Add(module);
            }
            moduleRepository.Save(editedModule);
        }
    }

}