using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        readonly ISessionFactory sessionFactory;
        public InvoiceRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<Invoice> GetAll()
        {
            using(var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Invoice>().List<Invoice>();
            }
        }



        public IList<Invoice> GetByCompanyId(int companyId)
        {
            using(var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Invoice>().
                    Add(Restrictions.Eq("CompanyId", companyId)).
                    List<Invoice>();
            }
        }

        public IList<Invoice> GetByDeleteStatus(bool isDelete)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Invoice>().
                    Add(Restrictions.Eq("isDeleted", isDelete)).
                    List<Invoice>();
            }
        }

        public Invoice GetById(int id)
        {
            using(var session = sessionFactory.OpenSession())
            {
                return session.Get<Invoice>(id);
            }
        }

        public IList<Invoice> GetByIds(int[] ids)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<Invoice>().
                    Add(Restrictions.In(Projections.Id(), ids)).
                    List<Invoice>();
                return result;

            }
        }

        public int Save(Invoice invoice)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(invoice);
                    transaction.Commit();
                    return invoice.Id;
                }
            }
        }

        public void Save(IList<Invoice> invoices)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    foreach(Invoice invoice in invoices)
                    {
                        session.SaveOrUpdate(invoice);
                    }
                    transaction.Commit();
                }
            }
        }
    }
}