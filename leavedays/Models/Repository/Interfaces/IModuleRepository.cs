using leavedays.Models.ViewModels.License;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IModuleRepository
    {
        IEnumerable<int> Save(IEnumerable<Module> modules);
        int Save(Module user);
        Module GetById(int id);
        IList<Module> GetById(IEnumerable<int> ids);
        IList<Module> GetAll();
        IList<Module> GetByLicenseId(int licensId, bool? isActive =  null);
        IList<Module> GetByLockStatus(int licenseId, bool lockStatus);
        IList<ModuleForDownload> GetForDownload(int[] moduleId, bool ignoreLockedStatus);
        void Delete(Module module);
    }
}
