using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models.ViewModel;
using NHibernate.Transform;

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

        public IList<LicenseInfo> GetLicenseInformation()
        {
            using (var session = sessionFactory.OpenSession())
            {
                string sqlQuery = string.Format(@"SELECT (u.FirstName+u.LastName)AS ContactPerson, u.UserName AS Email, u.PhoneNumber, c.FullName AS CompanyName, l.LicenseId, l.LicenseCode AS LicenceCode ");
                sqlQuery = string.Concat(sqlQuery, "FROM AppUser as u INNER JOIN Company AS c ON u.CompanyId = c.CompanyId INNER JOIN License AS l ON c.LicenseId = l.LicenseId INNER JOIN User_Role AS ur ON u.UserId = ur.UserId ");
                sqlQuery = string.Concat(sqlQuery, "WHERE ur.RoleId = 3");
                var licenses = session.CreateSQLQuery(sqlQuery).
                    SetResultTransformer(Transformers.AliasToBean<LicenseInfo>()).
                    List<LicenseInfo>();
                return licenses;
            }
        }

        public IList<LicenseInfo> GetSearchedInformation(string searchedLine)
        {
            using (var session = sessionFactory.OpenSession())
            {
                string sqlQuery = (@"SELECT(u.FirstName + u.LastName)AS ContactPerson, u.UserName AS Email, u.PhoneNumber, c.FullName AS CompanyName, l.LicenseId, l.LicenseCode AS LicenceCode ");
                sqlQuery = string.Concat(sqlQuery, "FROM AppUser as u INNER JOIN Company AS c ON u.CompanyId = c.CompanyId INNER JOIN License AS l ON c.LicenseId = l.LicenseId INNER JOIN User_Role AS ur ON u.UserId = ur.UserId ");
                sqlQuery = string.Concat(sqlQuery, "WHERE (ur.RoleId = 3) AND ((u.FirstName LIKE '%{0}%') OR (u.LastName LIKE '%{0}%') OR (u.PhoneNumber LIKE '%{0}%') OR (u.UserName LIKE '%{0}%') OR (c.FullName LIKE '%{0}%') OR (l.LicenseCode LIKE '%{0}%'))");
                sqlQuery = string.Format(sqlQuery, searchedLine);
                var licenses = session.CreateSQLQuery(sqlQuery).
                    SetResultTransformer(Transformers.AliasToBean<LicenseInfo>()).
                    List<LicenseInfo>();
                return licenses;
            }
        }

        public IList<LicenseInfo> GetAdwenchedSearchedInformation(SearchOption option)
        {
            using (var session = sessionFactory.OpenSession())
            {
                string sqlQuery = (@"SELECT(u.FirstName + u.LastName)AS ContactPerson, u.UserName AS Email, u.PhoneNumber, c.FullName AS CompanyName, l.LicenseId, l.LicenseCode AS LicenceCode ");
                sqlQuery = string.Concat(sqlQuery, "FROM AppUser as u INNER JOIN Company AS c ON u.CompanyId = c.CompanyId INNER JOIN License AS l ON c.LicenseId = l.LicenseId INNER JOIN User_Role AS ur ON u.UserId = ur.UserId ");
                sqlQuery = string.Concat(sqlQuery, "WHERE (ur.RoleId = 3) AND (((u.FirstName + u.LastName) LIKE '%{0}%') AND (u.PhoneNumber LIKE '%{1}%') AND (u.UserName LIKE '%{2}%') AND (c.FullName LIKE '%{3}%') AND (l.LicenseCode LIKE '%{4}%'))");
                sqlQuery = string.Format(sqlQuery, option.ContactPerson, option.PhoneNumber, option.Email, option.CompanyName, option.LicenceCode);
                var licenses = session.CreateSQLQuery(sqlQuery).
                    SetResultTransformer(Transformers.AliasToBean<LicenseInfo>()).
                    List<LicenseInfo>();
                return licenses;
            }
        }

        public int Save(License license)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.SaveOrUpdate(license);
                    t.Commit();
                    return license.Id;
                }
            }
        }

        public IList<License> GetByDefaultLicenseId(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<License>().
                    Add(Restrictions.Eq("DefaultLicenseId", id)).
                    List<License>();
            }
        }

        public IList<License> GetByDefaultLicenseIds(int[] id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<License>().
                    Add(Restrictions.In("DefaultLicenseId", id)).
                    List<License>();
            }
        }
    }
}