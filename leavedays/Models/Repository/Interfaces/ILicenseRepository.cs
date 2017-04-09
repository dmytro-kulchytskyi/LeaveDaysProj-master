using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface ILicenseRepository
    {
        IList<License> GetAll();
        int Save(License license);
        License GetById(int id);
        License GetByName(string name);
    }
}
