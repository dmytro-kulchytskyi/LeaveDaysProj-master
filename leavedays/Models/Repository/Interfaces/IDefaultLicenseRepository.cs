using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IDefaultLicenseRepository
    {
        IList<DefaultLicense> GetAll();
        int Save(DefaultLicense license);
        DefaultLicense GetById(int id);
        DefaultLicense GetByName(string name);
        IList<DefaultLicense> GetByModuleId(int id);
        void Save(List<DefaultLicense> licenses);
        IList<DefaultLicense> GetByNames(List<string> names);
    }
}
