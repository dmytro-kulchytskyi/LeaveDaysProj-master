using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class LicenseRepository : ILicenseRepository
    {
        readonly ISessionFactory sessionFactory;

        public LicenseRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<License> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                var results = session.CreateCriteria<License>().List<License>();
                return results;
            }
        }

        public License GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<License>(id);
            }
        }


        public int Save(License license)
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