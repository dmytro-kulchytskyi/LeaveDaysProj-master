using leavedays.Models;
using leavedays.Models.EditModel;
using leavedays.Models.ViewModel;
using leavedays.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModels.License;
using System.Text;

namespace leavedays.Controllers
{
    public class ModuleController : Controller
    {
        private readonly CompanyService companyService;
        private readonly InvoiceService invoiceService;
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly LicenseService licenseService;
        private readonly IDefaultModuleRepository defaultModuleRepository;
        private readonly IDefaultLicenseRepository defaultLicenseRepository;
        private readonly RequestService requestService;
        private readonly UserManager<AppUser, int> userManager;


        public ModuleController(RequestService requestService,
           UserManager<AppUser, int> userManager,
           CompanyService companyService,
           IUserRepository userRepository,
           LicenseService licenseService,
           ILicenseRepository licenseRepository,
           ICompanyRepository companyRepository,
           IInvoiceRepository invoiceRepository,
           IModuleRepository moduleRepository,
           InvoiceService invoiceService,
           IDefaultModuleRepository defaultModuleRepository,
           IDefaultLicenseRepository defaultLicenseRepository)
        {
            this.licenseService = licenseService;
            this.userRepository = userRepository;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
            this.invoiceService = invoiceService;
            this.defaultModuleRepository = defaultModuleRepository;
            this.defaultLicenseRepository = defaultLicenseRepository;
            this.userManager = userManager;
            this.requestService = requestService;
        }


        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (currentUser == null) return RedirectToAction("Index", "Home");
            EditRequest request = new EditRequest()
            {
                Status = "New",
                UserId = currentUser.Id,
                CompanyId = currentUser.CompanyId,
            };
            return View(request);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(EditRequest request)
        {
            if (!ModelState.IsValid) RedirectToAction("Index", "Home");
            requestService.Save(request);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> ConfirmNew()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if(User.IsInRole("customer") && User.IsInRole("manager"))
            {
                return View("RequestPanel", requestService.GetInProgressRequest(currentUser.CompanyId).OrderBy(model => model.IsAccepted));
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Accept(int Id, string returnUrl = "")
        {
            requestService.Accept(Id);
            if (string.IsNullOrEmpty(returnUrl)) return View("Index", "Home");
            return Redirect(returnUrl);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Reject(int Id, string returnUrl = "")
        {
            requestService.Reject(Id);
            if (string.IsNullOrEmpty(returnUrl)) return View("Index", "Home");
            return Redirect(returnUrl);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> ShowConfirmed()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (User.IsInRole("customer") && User.IsInRole("manager"))
            {
                return View("ConfirmedRequest", requestService.GetConfirmedRequest(currentUser.CompanyId).OrderBy(model => model.IsAccepted));
            }
            return View("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> ShowSended()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (currentUser == null) return RedirectToAction("Index", "Home");
            return View("UsersRequest", requestService.GetSendedByUserId(currentUser.Id));
        }

        public ActionResult eOwerview()
        {
            return View();
        }
        public ActionResult cOverview()
        {
            return View();
        }
        public ActionResult Pending()
        {
            return View();
        }

        [Authorize(Roles = "customer")]
        [HttpGet]
        public async Task<ActionResult> licenceInfo()
        {
            var user = userRepository.GetById(User.Identity.GetUserId<int>());
            if (user == null) return RedirectToAction("Index", "Home");

            var companyId = user.CompanyId;
            var company = companyRepository.GetById(companyId);
            var licenseId = company.LicenseId;
            var license = licenseRepository.GetById(licenseId);
            var disabledModules = moduleRepository.GetByLicenseId(licenseId, false);
            var defaultModules = disabledModules.Select(module => defaultModuleRepository.GetById(module.Id)).ToList();

            var model = new EditLicenseModules()
            {
                LicenseName = defaultLicenseRepository.GetById(license.DefaultLicenseId).Name,
                LicenseCode = license.LicenseCode,
                Modules = defaultModules
            };


            return View(model);
        }
    }
}