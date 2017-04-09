using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class DefaultLicenseRepository : IDefaultLicenseRepository
    {
        readonly ISessionFactory sessionFactory;

        public DefaultLicenseRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<DefaultLicense> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                var results = session.CreateCriteria<DefaultLicense>().List<DefaultLicense>();
                return results;
            }
        }

        public DefaultLicense GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<DefaultLicense>(id);
            }
        }

        public DefaultLicense GetByName(string name)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<DefaultLicense>()
                    .Add(Restrictions.Eq("Name", name))
                    .UniqueResult<DefaultLicense>();
                return result;
            }
        }

        public int Save(DefaultLicense license)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.Save(license);
                    t.Commit();
                    return license.Id;
                }

            }
        }

     }
    }
}