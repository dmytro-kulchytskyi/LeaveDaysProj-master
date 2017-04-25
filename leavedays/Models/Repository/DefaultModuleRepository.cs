using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;
using leavedays.Models.Repository.Interfaces;

namespace leavedays.Models.Repository
{
    public class DefaultModuleRepository : IDefaultModuleRepository
    {
        readonly ISessionFactory sessionFactory;

        public DefaultModuleRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<DefaultModule> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<DefaultModule>().List<DefaultModule>();
            }
        }

        public DefaultModule GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<DefaultModule>(id);
            }
        }

        public DefaultModule GetByName(string name)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<DefaultModule>().Add(Restrictions.Eq("Name", name)).UniqueResult<DefaultModule>();
            }
        }

        public int Save(DefaultModule module)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.SaveOrUpdate(module);
                    t.Commit();
                    return module.Id;
                }
            }
        }
    }
}
