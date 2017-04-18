using leavedays.Models;
using leavedays.Models.Repository;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModels.Account;
using leavedays.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{

    public class AccountController : Controller
    {
        private readonly string[] CreateUserAllowedRoles = new string[] { "worker", "manager" };


        private readonly CompanyService companyService;
        private readonly UserManager<AppUser, int> userManager;
        private readonly SignInManager<AppUser, int> signInManager;
        private readonly ICompanyRepository companyRepository;
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly IDefaultLicenseRepository defaultLicenseRepository;


        public AccountController(
            UserManager<AppUser, int> userManager,
            SignInManager<AppUser, int> signInManager,
            CompanyService companyService,
            ICompanyRepository companyRepository,
           IUserRepository userRepository,
           ILicenseRepository licenseRepository,
           IDefaultLicenseRepository defaultLicenseRepository)
        {
            this.defaultLicenseRepository = defaultLicenseRepository;
            this.userRepository = userRepository;
            this.companyRepository = companyRepository;
            this.companyService = companyService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.licenseRepository = licenseRepository;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


      
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var user = new AppUser { UserName = model.UserName, Password = model.Password };
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        var companyId = userRepository.GetByUserName(model.UserName).CompanyId;
                        if (companyId == 0) return RedirectToAction("Index", "Home");
                        var company = companyRepository.GetById(companyId).UrlName;
                        if (string.IsNullOrEmpty(company)) return RedirectToAction("Index", "Home");
                        return RedirectToAction("Company", "Account", new { companyName = company });
                    }
                //case SignInStatus.LockedOut:
                //    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = "", RememberMe = model.RememberMe });
                //case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        public string Info()
        {
            return "Is Customer: " + User.IsInRole("customer") + "<br /> Is Auth: " + User.Identity.IsAuthenticated;
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            var licenseList = defaultLicenseRepository.GetAll();
            var model = new RegisterViewModel();
            model.LicenseList = licenseList;
            //  model.Roles = CreateUserAllowedRoles;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.LicenseList = defaultLicenseRepository.GetAll();
                return View(model);

            }

            model.CompanyUrl = model.CompanyUrl.ToLower();

            var isUniq = companyRepository.GetByUrlName(model.CompanyUrl) == null;
            if (!isUniq)
            {
                ModelState.AddModelError("", "A company with this URL already exists");
                return View(model);
            }

            var license = companyService.CreateLicense(defaultLicenseRepository.GetByName(model.LicenseName));



            List<string> rolesList = new List<string>();
            if (string.IsNullOrEmpty(model.RolesLine))
                rolesList.Add(CreateUserAllowedRoles[0]);

            rolesList = CreateUserAllowedRoles.ToList();

            if (rolesList.Count == 0 || !rolesList.Contains("customer"))
                rolesList.Add("customer");


            var company = new Company()
            {
                FullName = model.CompanyName,
                UrlName = model.CompanyUrl,
                LicenseId = license.Id
            };
            var companyId = companyRepository.Save(company);

            var userRoles = companyService.GetRolesList(rolesList);

            var user = new AppUser()
            {
                UserName = model.UserName,
                Roles = new HashSet<Role>(userRoles),
                CompanyId = companyId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }
            await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToAction("Index", "Home");
        }

        // [Authorize(Roles="Customer")]
        [HttpGet]
        [Authorize(Roles = "customer")]
        public ActionResult CreateEmployee()
        {
            // if (!userManager.IsInRole(User.Identity.GetUserId<int>(), "customer")) return HttpNotFound();
            var model = new CreateEmployeeViewModel();
            model.Roles = CreateUserAllowedRoles;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            model.Roles = model.Roles = CreateUserAllowedRoles;
            if (!ModelState.IsValid)
            {
                model.Password = "";
                return View(model);
            }


            List<string> rolesList = new List<string>();
            if (string.IsNullOrEmpty(model.RolesLine))
                rolesList.Add(CreateUserAllowedRoles[0]);

            rolesList = model.RolesLine.SplitByComma().Select(r => r.ToLower()).Intersect(CreateUserAllowedRoles).ToList();
            if (rolesList.Count == 0)
                rolesList.Add(CreateUserAllowedRoles[0]);

            var userRoles = companyService.GetRolesList(rolesList);

            var user = new AppUser()
            {
                UserName = model.UserName,
                Roles = new HashSet<Role>(userRoles),
                CompanyId = userRepository.GetById(User.Identity.GetUserId<int>()).CompanyId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,

            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "financeadmin")]
        public ActionResult CreateCompany()
        {
            var model = new CreateCompanyViewModel();
            model.Roles = CreateUserAllowedRoles;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateCompany(CreateCompanyViewModel model)
        {

            if (!userManager.IsInRole(User.Identity.GetUserId<int>(), "financeadmin")) return HttpNotFound();
            model.Roles = model.Roles = CreateUserAllowedRoles;
            if (!ModelState.IsValid) return View(model);

            model.CompanyUrl = model.CompanyUrl.ToLower();

            var isUniq = companyRepository.GetByUrlName(model.CompanyUrl) == null;
            if (!isUniq)
            {
                ModelState.AddModelError("", "A company with this URL already exists");
                return View(model);
            }

            List<string> rolesList = new List<string>();
            if (string.IsNullOrEmpty(model.RolesLine))
                rolesList.Add(CreateUserAllowedRoles[0]);

            rolesList = model.RolesLine.SplitByComma()
                .Select(r => r.ToLower())
                .Intersect(CreateUserAllowedRoles).ToList();

            if (rolesList.Count == 0 || !rolesList.Contains("customer"))
                rolesList.Add("customer");

            var company = new Company()
            {
                FullName = model.CompanyName,
                UrlName = model.CompanyUrl
            };
            var companyId = companyRepository.Save(company);

            var userRoles = companyService.GetRolesList(rolesList);

            var user = new AppUser()
            {
                UserName = model.UserName,
                Roles = new HashSet<Role>(userRoles),
                CompanyId = companyId,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Password = model.Password
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Company(string companyName = "")
        {
            if (string.IsNullOrEmpty(companyName)) return RedirectToAction("Index", "Home");
            var company = companyRepository.GetByUrlName(companyName);
            if (company == null) return RedirectToAction("Index", "Home");
            ViewBag.CompanyName = company.FullName;
            return View(userRepository.GetByCompanyId(company.Id));
        }

        [Authorize]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}