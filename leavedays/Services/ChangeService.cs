using leavedays.App_Start;
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
        private readonly IModuleRepository moduleRepository;
        private readonly IModuleChangeRepository moduleChangeRepository;
        private readonly ILicenseRepository licenseRepository;

        public static ChangeService Instance
        {
            get { return (ChangeService)NinjectWebCommon.bootstrapper.Kernel.GetService(typeof(ChangeService)); }
        }

        public ChangeService(
          IModuleRepository moduleRepository,
          IModuleChangeRepository moduleChangeRepository,
          ILicenseRepository licenseRepository)
        {
            this.moduleRepository = moduleRepository;
            this.moduleChangeRepository = moduleChangeRepository;
            this.licenseRepository = licenseRepository;
        }

        public void ApplyChanges()
        {
            var currentDate = DateTime.Now;
            var moduleNeedToChange = moduleChangeRepository.GetByDate(currentDate.Year, currentDate.Month, null);
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

        public void LockLicense()
        {
            var unpaidLicense = licenseRepository.GetByPaidStatus(false);
            foreach(var license in unpaidLicense)
            {
                license.IsLocked = true;
            }
            licenseRepository.Save(unpaidLicense);
        }
    }

}