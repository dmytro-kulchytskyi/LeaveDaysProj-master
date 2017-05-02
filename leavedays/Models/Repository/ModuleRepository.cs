using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModels.License;

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

        

        public IList<Module> GetByLockStatus(int licenseId, bool lockStatus)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Module>().
                    Add(Restrictions.Eq("IsLocked", lockStatus)).
                    List<Module>();
                    
            }
        }

        public IList<Module> GetByLicenseId(int licensId, bool? isActive = null)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<Module>()
                    .Add(Restrictions.Eq("LicenseId", licensId));
                if (isActive.HasValue)
                {
                   result.Add(Restrictions.Eq("IsActive", isActive));
                }
                return result.List<Module>();
            }
        }

        public int Save(Module module)
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

        public IEnumerable<int> Save(IEnumerable<Module> modules)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    var ids = new HashSet<int>();
                    foreach (var m in modules)
                    {
                        session.SaveOrUpdate(m);
                        ids.Add(m.Id);
                    }
                    t.Commit();
                    return ids;
                }
            }
        }

        public void Delete(Module module)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.Delete(module);
                    t.Commit();
                }
            }
        }

        public IList<ModuleForDownload> GetForDownload(int[] moduleId, bool ignorLockStatus)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var sqlQuery = "SELECT m.ModuleId AS Id, m.Price , d.Name FROM Module AS m INNER JOIN DefaultModule AS d ON m.DefaultModuleId  = d.DefaultModuleId WHERE m.IsActive = 1 AND m.ModuleId IN(" + string.Join(",", moduleId) + ") ";
                if(!ignorLockStatus)
                {
                    sqlQuery += "AND m.IsLocked = 0";
                }
                var res = session.CreateSQLQuery(sqlQuery);
                return res.
                    SetResultTransformer(Transformers.AliasToBean<ModuleForDownload>()).
                    List<ModuleForDownload>();
            }
        }

        public IList<Module> GetById(IEnumerable<int> ids)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var modules = ids.Select(id => session.Get<Module>(id)).ToList();
                return modules;
            }
        }
    }
}
