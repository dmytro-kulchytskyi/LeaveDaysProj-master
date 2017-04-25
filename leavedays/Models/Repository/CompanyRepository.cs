using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        readonly ISessionFactory sessionFactory;
        public CompanyRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }



        public IList<Company> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Company>().List<Company>();
            }
        }

        public Company GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var company = session.Get<Company>(id);
                return company;
            }
        }

        public Company GetByUrlName(string urlName)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Company>().Add(Restrictions.Eq("UrlName", urlName.ToLower())).UniqueResult<Company>();
            }
        }

        public int GetOwnerId(int companyId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Company>()
                      .SetProjection(Projections.Property("Owner"))
                      .Add(Restrictions.IdEq(companyId))
                      .UniqueResult<int>();
            }
        }
        public IList<Company> GetByCompanyIds(IList<int> companyIds)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var companys = session.CreateCriteria<Company>().
                    Add(Restrictions.In("Id", companyIds.ToArray<int>())).
                    List<Company>();
                return companys;
            }
        }

        public int Save(Company user)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.SaveOrUpdate(user);
                    t.Commit();
                    return user.Id;
                }
            }
        }

        public int GetUsersCount(int companyId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<AppUser>()
                    .SetProjection(Projections.RowCount())
                    .Add(Restrictions.Eq("CompanyId", companyId))
                    .UniqueResult<int>();
                return result;
            }
        }
    }
}