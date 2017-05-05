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
    public class UserRepository : IUserRepository
    {
        readonly ISessionFactory sessionFactory;

        public UserRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<AppUser> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<AppUser>().
                    List<AppUser>();
            }
        }

        public IList<AppUser> GetByCompanyId(int companyId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var userList = session.CreateCriteria<AppUser>()
                    .Add(Restrictions.Eq("CompanyId", companyId)).List<AppUser>();
                return userList;
            }
        }

        public AppUser GetById(int id)
        {
            using (var sesson = sessionFactory.OpenSession())
            {
                return sesson.Get<AppUser>(id);
            }
        }

        public AppUser GetOwnerByCompanyId(int companyId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                AppUser owner = session.CreateCriteria<AppUser>().
                    CreateAlias("Roles", "roles").
                    Add(Restrictions.Eq("roles.Name", "customer")).
                    Add(Restrictions.Eq("CompanyId", companyId)).UniqueResult<AppUser>();
                return owner;
            }
        }

        public IList<AppUser> GetOwnersByCompanyIds(IList<int> companyId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var owners = session.CreateCriteria<AppUser>().
                    CreateAlias("Roles", "roles").
                    Add(Restrictions.Eq("roles.Name", "customer")).
                    Add(Restrictions.In("CompanyId", companyId.ToArray<int>())).
                    List<AppUser>();
                return owners;
            }
        }

        public AppUser GetByUserName(string userName)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var user = session.CreateCriteria<AppUser>()
                    .Add(Restrictions.Eq("UserName", userName))
                    .UniqueResult<AppUser>();
                return user;
            }
        }

        public int Save(AppUser user)
        {
            using (var sesson = sessionFactory.OpenSession())
            {
                using (var t = sesson.BeginTransaction())
                {
                    sesson.SaveOrUpdate(user);
                    t.Commit();
                    return user.Id;
                }
            }
        }

        public IList<AppUser> GetCustomers()
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<AppUser>().
                     CreateAlias("Roles", "roles").
                    Add(Restrictions.Eq("roles.Name", "customer")).
                    List<AppUser>();
            }
        }

        public int Delete(AppUser user)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.Delete(user);
                    return user.Id;
                    t.Commit();
                }
            }
        }
       
    }
}
