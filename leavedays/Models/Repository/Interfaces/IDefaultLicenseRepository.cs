using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    interface IDefaultLicenseRepository
    {
        IList<DefaultLicense> GetAll();
        int Save(DefaultLicense license);
        DefaultLicense GetById(int id);
        DefaultLicense GetByName(string name);
    }
}
