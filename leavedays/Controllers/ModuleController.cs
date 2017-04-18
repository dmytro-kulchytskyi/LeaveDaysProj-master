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

namespace leavedays.Controllers
{
    public class ModuleController : Controller
    {
        readonly CompanyService companyService;
        readonly RequestService requestService;
        private readonly UserManager<AppUser, int> userManager;

        public ModuleController(RequestService requestService,
            UserManager<AppUser, int> userManager,
            CompanyService companyService)
        {
            this.requestService = requestService;
            this.userManager = userManager;
            this.companyService = companyService;
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

        [Authorize]
        public async Task<ActionResult> licenceInfo()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (currentUser == null) return RedirectToAction("Index", "Home");
            return View();
        }
    }
}