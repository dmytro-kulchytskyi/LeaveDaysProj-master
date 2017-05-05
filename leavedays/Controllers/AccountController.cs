using leavedays.App_Start;
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
        private readonly string[] EmployeeRoles = new string[]
        {
            Roles.Worker,
            Roles.Manager
        };

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


        private IAuthenticationManager authenticationManager
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
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
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
            var user = userRepository.GetByUserName(model.UserName);

            if (user != null)
            {
                var isPasswordCorrect = await userManager.CheckPasswordAsync(user, model.Password);

                if (isPasswordCorrect)
                {
                    if (user != null)
                    {
                        var company = companyRepository.GetById(user.CompanyId);

                        if (company != null)
                        {
                            var license = licenseRepository.GetById(company.LicenseId);
                            if (license != null && license.IsLocked)
                            {
                                ModelState.AddModelError("", "Account is locked, please pay the invoice");
                                return View(model);
                            }
                        }
                        await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: model.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            var licenseList = defaultLicenseRepository.GetAll();
            var model = new CreateUserModel();
            model.LicenseList = licenseList;
            //  model.Roles = CreateUserAllowedRoles;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Register(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                model.LicenseList = defaultLicenseRepository.GetAll();
                return View(model);
            }

            var createCompanyResult = companyService.CreateCompany(model.CompanyName, model.CompanyUrl, model.LicenseName);
            if (!createCompanyResult.Succed)
            {
                ModelState.AddModelError("", createCompanyResult.GetMessage());
                model.LicenseList = defaultLicenseRepository.GetAll();
                return View(model);
            }
            var company = createCompanyResult.GetResult();

            var userInfo = new CreateUser()
            {
                UserName = model.UserName,
                Role = Roles.Customer,
                Comapany = company,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email
            };

            var createUserResult = await companyService.CreateUserAsync(userInfo, userManager);

            if (!createUserResult.Succed)
            {
                ModelState.AddModelError("", createUserResult.GetMessage());
                model.LicenseList = defaultLicenseRepository.GetAll();
                return View(model);
            }
            var user = createUserResult.GetResult();
            await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToAction("Index", "Home");
        }

        // [Authorize(Roles="Customer")]
        [HttpGet]
        [Authorize(Roles = Roles.Customer)]
        public ActionResult CreateEmployee()
        {
            var model = new CreateEmployeeViewModel();
            model.Roles = EmployeeRoles;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Customer)]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            model.Roles = EmployeeRoles;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = userRepository.GetById(User.Identity.GetUserId<int>());

            var company = companyRepository.GetById(customer.CompanyId);
            var license = licenseRepository.GetById(company.LicenseId);

            var activeUsers = companyRepository.GetUsersCount(company.Id);

            if (license.Seats - activeUsers <= 0)
            {
                ModelState.AddModelError("", "No free seats");
                return View(model);
            }

            if (customer == null)
            {
                ModelState.AddModelError("", "Error while creating employee");
                return View(model);
            }

            var rolesList = model.RolesLine.SplitByComma().Intersect(EmployeeRoles, StringComparer.OrdinalIgnoreCase).ToList();

            var userRoles = companyService.GetRolesList(rolesList);
            if (userRoles == null || userRoles.Count() == 0)
            {
                ModelState.AddModelError("", "You must select a role(s)");
                return View(model);
            }

            var user = new AppUser()
            {
                UserName = model.UserName,
                Roles = new HashSet<Role>(userRoles),
                CompanyId = customer.CompanyId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                Email = model.Email,
                PhoneNumber = model.Phone
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating employee");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = Roles.FinanceAdmin)]
        public ActionResult CreateCompany()
        {
            var model = new CreateUserModel();
            model.Roles = EmployeeRoles;
            model.LicenseList = defaultLicenseRepository.GetAll();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateCompany(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                model.LicenseList = defaultLicenseRepository.GetAll();
                return View(model);
            }

            var createCompanyResult = companyService.CreateCompany(model.CompanyName, model.CompanyUrl, model.LicenseName);
            if (!createCompanyResult.Succed)
            {
                ModelState.AddModelError("", createCompanyResult.GetMessage());
                model.LicenseList = defaultLicenseRepository.GetAll();
                return View(model);
            }
            var company = createCompanyResult.GetResult();

            var userInfo = new CreateUser()
            {
                UserName = model.UserName,
                Role = Roles.Customer,
                Comapany = company,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email
            };

            var createUserResult = await companyService.CreateUserAsync(userInfo, userManager);

            if (!createUserResult.Succed)
            {
                ModelState.AddModelError("", createUserResult.GetMessage());
                model.LicenseList = defaultLicenseRepository.GetAll();
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
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}