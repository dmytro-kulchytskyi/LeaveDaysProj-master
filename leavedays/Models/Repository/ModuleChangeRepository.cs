using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
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

        public int Delete(ModuleChange module)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    var id = module.Id;
                    session.Delete(module);
                    t.Commit();
                    return module.Id;
                }
            }
        }

        public IList<ModuleChange> GetByDate(int year, int month, int? moduleId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var query = string.Format(@"SELECT * FROM ModuleChange Where YEAR(StartDate) = :param1 AND MONTH(StartDate) = :param2 ");
                if(moduleId != null)
                {
                    query += string.Join(query, "AND ModuleId = :param3");
                }
                var result =  session.CreateSQLQuery(query).
                    SetParameter("param1", year).
                    SetParameter("param2", month);
                if(moduleId != null)
                {
                    result.SetParameter("param3", moduleId);
                }
                return result.SetResultTransformer(Transformers.AliasToBean<ModuleChange>()).
                    List<ModuleChange>();
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