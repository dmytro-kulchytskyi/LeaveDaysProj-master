using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IUserRepository
    {
        int Save(AppUser user);
        AppUser GetById(int id);
        AppUser GetByUserName(string userName);
        IList<AppUser> GetAll();
        IList<AppUser> GetByCompanyId(int companyId);
        AppUser GetOwnerByCompanyId(int companyId);
        IList<AppUser> GetOwnersByCompanyIds(IList<int> companyId);
        IList<AppUser> GetCustomers();
        int Delete(AppUser user);
    }
}
