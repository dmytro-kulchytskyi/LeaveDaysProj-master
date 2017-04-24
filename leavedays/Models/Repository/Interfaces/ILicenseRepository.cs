using leavedays.Models.ViewModel;
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
        IList<LicenseInfo> GetLicenseInformation();
        IList<LicenseInfo> GetSearchedInformation(string searchedLine);
        IList<LicenseInfo> GetAdwenchedSearchedInformation(SearchOption option);
    }
}
