using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IModuleChangeRepository
    {
        int Save(ModuleChange module);
        ModuleChange GetById(int id);
        IList<ModuleChange> GetByModuleId(int id);
        void Save(List<ModuleChange> modules);
        IList<ModuleChange> GetByDate(int year, int month, int? moduleId);
        int Delete(ModuleChange module);
    }
}