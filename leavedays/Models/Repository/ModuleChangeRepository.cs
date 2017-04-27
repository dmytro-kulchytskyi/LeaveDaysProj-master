using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class ModuleChangeRepository : IModuleChangeRepository
    {
        readonly ISessionFactory sessionFactory;
        public ModuleChangeRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public ModuleChange GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<ModuleChange>(id);
            }
        }

        public IList<ModuleChange> GetByModuleId(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<ModuleChange>().
                    Add(Restrictions.Eq("ModuleId", id)).
                    List<ModuleChange>();
            }
        }

        public int Save(ModuleChange module)
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

        public void Save(List<ModuleChange> modules)
        {
            using (var session = sessionFactory.OpenSession())
            using (var t = session.BeginTransaction())
            {
                foreach (ModuleChange module in modules)
                {
                    session.SaveOrUpdate(module);
                }
                t.Commit();
            }
        }
    }
}