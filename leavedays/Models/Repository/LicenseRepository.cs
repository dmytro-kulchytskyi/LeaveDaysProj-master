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
                string sqlQuery = string.Format(@"SELECT (u.FirstName+u.LastName)AS ContactPerson, u.UserId, u.Email AS Email, u.PhoneNumber, c.IsPaid, c.FullName AS CompanyName, l.LicenseId, l.LicenseCode AS LicenceCode ");
                sqlQuery = string.Concat(sqlQuery, "FROM AppUser as u INNER JOIN Company AS c ON u.CompanyId = c.CompanyId INNER JOIN License AS l ON c.LicenseId = l.LicenseId INNER JOIN User_Role AS ur ON u.UserId = ur.UserId INNER JOIN Role AS r ON ur.RoleId = r.RoleId ");
                sqlQuery = string.Concat(sqlQuery, "WHERE r.Name = 'customer'");
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
                string sqlQuery = (@"SELECT(u.FirstName + u.LastName)AS ContactPerson, u.UserId, u.Email AS Email, u.PhoneNumber, c.IsPaid, c.FullName AS CompanyName, l.LicenseId, l.LicenseCode AS LicenceCode ");
                sqlQuery = string.Concat(sqlQuery, "FROM AppUser as u INNER JOIN Company AS c ON u.CompanyId = c.CompanyId INNER JOIN License AS l ON c.LicenseId = l.LicenseId INNER JOIN User_Role AS ur ON u.UserId = ur.UserId INNER JOIN Role AS r ON ur.RoleId = r.RoleId ");
                sqlQuery = string.Concat(sqlQuery, "WHERE (r.Name = 'customer') AND ((u.FirstName LIKE :param) OR (u.LastName LIKE :param) OR (u.PhoneNumber LIKE :param) OR (u.UserName LIKE :param) OR (c.FullName LIKE :param) OR (l.LicenseCode LIKE :param))");
                var licenses = session.CreateSQLQuery(sqlQuery).
                    SetParameter("param", "%" + searchedLine + "%").
                    SetResultTransformer(Transformers.AliasToBean<LicenseInfo>()).
                    List<LicenseInfo>();
                return licenses;
            }
        }

        public IList<LicenseInfo> GetAdwenchedSearchedInformation(SearchOption option)
        {
            using (var session = sessionFactory.OpenSession())
            {
                string sqlQuery = (@"SELECT(u.FirstName + u.LastName)AS ContactPerson, u.UserId, u.Email AS Email, u.PhoneNumber, c.IsPaid, c.FullName AS CompanyName, l.LicenseId, l.LicenseCode AS LicenceCode ");
                sqlQuery = string.Concat(sqlQuery, "FROM AppUser as u INNER JOIN Company AS c ON u.CompanyId = c.CompanyId INNER JOIN License AS l ON c.LicenseId = l.LicenseId INNER JOIN User_Role AS ur ON u.UserId = ur.UserId INNER JOIN Role AS r ON ur.RoleId = r.RoleId ");
                sqlQuery = string.Concat(sqlQuery, "WHERE (r.Name = 'customer') AND (((u.FirstName + u.LastName) LIKE :name) AND (u.PhoneNumber LIKE :phone) AND (u.UserName LIKE :email) AND (c.FullName LIKE :company) AND (l.LicenseCode LIKE :license))");
                sqlQuery = string.Format(sqlQuery, option.ContactPerson, option.PhoneNumber, option.Email, option.CompanyName, option.LicenceCode);
                var licenses = session.CreateSQLQuery(sqlQuery).
                    SetParameter("name", "%" + option.ContactPerson + "%").
                    SetParameter("phone", "%" + option.PhoneNumber + "%").
                    SetParameter("email", "%" + option.Email + "%").
                    SetParameter("company", "%" + option.CompanyName + "%").
                    SetParameter("license", "%" + option.LicenceCode + "%").
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

        public void Delete(License license)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.Delete(license);
                    t.Commit();
                }
            }
        }

        public IList<License> GetByPaidStatus(bool status)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<License>().
                    Add(Restrictions.Eq("IsPaid", status)).
                    List<License>();
            }
        }

        public void Save(IList<License> licenses)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    foreach(var license in licenses)
                        session.SaveOrUpdate(license);
                    t.Commit();
                }
            }
        }
    }
}