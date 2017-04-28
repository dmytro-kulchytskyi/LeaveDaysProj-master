using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Role GetById(int id);
        Role GetByName(string name);
        IList<Role> GetByName(IEnumerable<string> names);
        int Save(Role group);
        IList<Role> GetAll();
    }
}
