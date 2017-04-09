using leavedays.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IRequestRepository
    {
        int Save(Request request);
        Request GetById(int id);
        IEnumerable<Request> GetByUserId(int userId);
        IEnumerable<Request> GetByRequestStatus(int companyId, params RequestStatus[] status);
        IEnumerable<Request> GetByCompanyId(int companyId);
    }
}