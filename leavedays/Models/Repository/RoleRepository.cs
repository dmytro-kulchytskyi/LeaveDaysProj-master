using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class RoleRepository : IRoleRepository
    {
        readonly ISessionFactory sessionFactory;

        public RoleRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<Role> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Role>().List<Role>();
            }
        }

        public Role GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<Role>(id);
            }
        }

        public Role GetByName(string name)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Role>()
                    .Add(Restrictions.Eq("Name", name))
                    .UniqueResult<Role>();
            }
        }

        public IList<Role> GetByName(IEnumerable<string> names)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<Role>()
                    .Add(Restrictions.In("Name", names.ToArray()))
                    .List<Role>();
                return result;
            }
        }

        public int Save(Role role)
        {

            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.SaveOrUpdate(role);
                    t.Commit();
                    return role.Id;
                }
            }
        }
    }
}