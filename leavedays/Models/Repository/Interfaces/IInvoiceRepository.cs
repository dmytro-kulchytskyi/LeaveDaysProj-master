using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface IInvoiceRepository
    {
        int Save(Invoice invoice);
        void Save(IList<Invoice> invoices);
        IList<Invoice> GetByDeleteStatus(bool isDelete);
        IList<Invoice> GetByIds(int[] ids);
        Invoice GetById(int id);
        IList<Invoice> GetAll();
        IList<Invoice> GetByCompanyId(int companyId);
    }
}
