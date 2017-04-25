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

        public IList<DefaultLicense> GetByModuleId(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<DefaultLicense>().
                    CreateAlias("DefaultModules", "module").
                    Add(Restrictions.Eq("module.Id", id)).
                    List<DefaultLicense>();
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

        public IList<DefaultLicense> GetByNames(List<string> names)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<DefaultLicense>().
                    Add(Restrictions.In("Name", names.ToArray())).
                    List<DefaultLicense>();
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

        public void Save(List<DefaultLicense> licenses)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    foreach(DefaultLicense license in licenses)
                    {
                        session.SaveOrUpdate(license);
                    }
                    t.Commit();
                }

            }
        }
    }
}
