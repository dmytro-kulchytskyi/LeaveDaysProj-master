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

        public IList<ModuleChange> GetByDate(int year, int month, int day)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var query = string.Format(@"SELECT * FROM ModuleChange Where YEAR(StartDate) = {0} AND MONTH(StartDate) = {1} AND DAY(StartDate) = {2}", year, month, day);
                return session.CreateSQLQuery(query).List<ModuleChange>();
            }
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
            {
                using (var t = session.BeginTransaction())
                {
                    foreach(ModuleChange module in modules)
                    {
                        session.SaveOrUpdate(module);
                    }
                    t.Commit();
                }
            }
        }
    }
}