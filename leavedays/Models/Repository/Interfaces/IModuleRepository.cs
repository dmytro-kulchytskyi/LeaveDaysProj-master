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
        int Save(Module module);
        IEnumerable<int> Save(IEnumerable<Module> modules);
        Module GetById(int id);
        IList<Module> GetAll();
        IList<Module> GetByLicenseId(int licensId, bool? isActive =  null);
    }
}
