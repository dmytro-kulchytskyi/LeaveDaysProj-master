using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface ICompanyRepository
    {
        int Save(Company user);
        Company GetById(int id);
        Company GetByUrlName(string urlName);
        IList<Company> GetAll();
        int GetOwnerId(int companyId);
        IList<Company> GetByCompanyIds(IList<int> companyIds);
        int GetUsersCount(int companyId);
    }
}
