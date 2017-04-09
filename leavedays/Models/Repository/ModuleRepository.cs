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
    public class ModuleRepository : IModuleRepository
    {
        readonly ISessionFactory sessionFactory;

        public ModuleRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<Module> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Module>().List<Module>();
            }
        }

        public Module GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<Module>(id);
            }
        }

        public Module GetByName(string name)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Module>().Add(Restrictions.Eq("Name", name)).UniqueResult<Module>();
            }
        }

        public int Save(Module module)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.Save(module);
                    t.Commit();
                    return module.Id;
                }
            }
        }
    }
}
