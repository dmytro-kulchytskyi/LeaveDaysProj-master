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
        IList<License> GetByPaidStatus(bool status);
        void Save(IList<License> licenses);
        IList<LicenseInfo> GetLicenseInformation();
        IList<LicenseInfo> GetSearchedInformation(string searchedLine);
        IList<LicenseInfo> GetAdwenchedSearchedInformation(SearchOption option);
        IList<License> GetByDefaultLicenseId(int id);
        IList<License> GetByDefaultLicenseIds(int[] id);
        void Delete(License license);
    }
}
