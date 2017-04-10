using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModel;
using leavedays.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private readonly CompanyService companyService;
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly ICompanyRepository companyRepository;


        public AdminController(
           CompanyService companyService,
           IUserRepository userRepository,
           ILicenseRepository licenseRepository,
           ICompanyRepository companyRepository)
        {
            this.userRepository = userRepository;
            this.companyService = companyService;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
        }



        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ShowLicensesInfo()
        {
            IList<Company> companys = companyRepository.GetAll();
            List<int> companyIds = companys.Select(m => m.Id).ToList();
            IList<AppUser> owners = userRepository.GetOwnersByCompanyIds(companyIds);
            var result = owners.Select(m => new LicenseInfo {
                CompanyName = companys.Where(n => n.Id == m.CompanyId).Select(n => n.FullName).First(),
                ContactPerson = m.FirstName + " " + m.LastName,
                Email = m.UserName,
                PhoneNumber = m.PhoneNumber,
                LicenseId = 1
            }).ToList();
            return View(result);
        }
    }
}