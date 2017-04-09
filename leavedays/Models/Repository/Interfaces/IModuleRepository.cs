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
        int Save(Module user);
        Module GetById(int id);
        Module GetByName(string name);
        IList<Module> GetAll();
    }
}
