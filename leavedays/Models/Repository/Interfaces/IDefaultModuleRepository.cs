using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IDefaultModuleRepository
    {
        int Save(DefaultModule user);
        DefaultModule GetById(int id);
        DefaultModule GetByName(string name);
        IList<DefaultModule> GetAll();
    }
}
