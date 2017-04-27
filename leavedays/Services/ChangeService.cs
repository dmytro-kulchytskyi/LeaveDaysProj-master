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

        public ChangeService(
          IModuleRepository moduleRepository,
          IModuleChangeRepository moduleChangeRepository)
        {
            this.moduleRepository = moduleRepository;
            this.moduleChangeRepository = moduleChangeRepository;
        }

        public  void ApplyChanges()
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