using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModel;
using leavedays.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using leavedays.Models.ViewModels.License;
using Microsoft.Win32;
using System.Text;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Hangfire;

namespace leavedays.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private readonly CompanyService companyService;
        private readonly InvoiceService invoiceService;
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly IModuleChangeRepository moduleChangeRepository;
        private readonly LicenseService licenseService;
        private readonly IDefaultModuleRepository defaultModuleRepository;
        private readonly IDefaultLicenseRepository defaultLicenseRepository;
        private readonly EmailSenderService emailService;


        public AdminController(
           IModuleChangeRepository moduleChangeRepository,
           CompanyService companyService,
           IUserRepository userRepository,
           LicenseService licenseService,
           ILicenseRepository licenseRepository,
           ICompanyRepository companyRepository,
           IInvoiceRepository invoiceRepository,
           IModuleRepository moduleRepository,
           InvoiceService invoiceService,
           IDefaultModuleRepository defaultModuleRepository,
           IDefaultLicenseRepository defaultLicenseRepository,
           EmailSenderService emailService)
        {
            this.emailService = emailService;
            this.moduleChangeRepository = moduleChangeRepository;
            this.licenseService = licenseService;
            this.userRepository = userRepository;
            this.companyService = companyService;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
            this.invoiceService = invoiceService;
            this.defaultModuleRepository = defaultModuleRepository;
            this.defaultLicenseRepository = defaultLicenseRepository;
        }



        public ActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult Invoices()
        {
            var invoices = invoiceService.GetInvoices();
            var result = invoices.Select(m => new InvoiceView
            {
                Id = m.Id,
                CompanyName = companyService.GetById(m.Company.Id).FullName,
                RecieveDate = m.RecieveDate
            });
            return View(result);
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpPost]
        public ActionResult DeleteInvoice(int id)
        {
            var invoice = invoiceRepository.GetById(id);
            invoice.isDeleted = true;
            invoiceRepository.Save(invoice);
            return RedirectToAction("Invoices");
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpPost]
        public ActionResult DeleteInvoices(string ids)
        {
            string[] idStringMass = ids.Split(' ');
            int[] idIntMass = new int[idStringMass.Length];
            for (int i = 0; i < idIntMass.Length; i++)
            {
                idIntMass[i] = int.Parse(idStringMass[i]);
            }
            var invoices = invoiceRepository.GetByIds(idIntMass);
            for (int i = 0; i < invoices.Count; i++)
            {
                invoices[i].isDeleted = true;
            }
            invoiceRepository.Save(invoices);
            return RedirectToAction("Invoices");
        }

        [Authorize]
        [HttpPost]
        public FileResult DownloadInvoice(int id)
        {
            InvoiceForDownload invoice = invoiceService.CreateInvoiceForDownload(id);
            var file = invoiceService.GetInvoiceFile(new List<InvoiceForDownload>() { invoice });
            return File(file, "text/csv", "Invoice" + id.ToString() + ".csv");
        }

        [Authorize]
        [HttpPost]
        public FileResult DownloadInvoices(string[] ids)
        {
            var invoices = invoiceService.CreateInvoicesForDownload(ids.Select(m => int.Parse(m)).ToArray());
            var file = invoiceService.GetInvoiceFile(invoices);
            return File(file, "text/csv", "Invoices.csv");
        }

        public ActionResult CreateTestInvoice()
        {
            var user = userRepository.GetById(User.Identity.GetUserId<int>());
            var invoice = new Invoice
            {
                Company = companyRepository.GetById(user.CompanyId),
                RecieveDate = DateTime.Now
            };
            invoiceRepository.Save(invoice);
            return Content(invoice.Id.ToString());
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult LicensesInfo()
        {
            return View("ShowLicensesInfo", licenseService.GetLicenseInfoList());
        }

        //-------
        [Authorize(Roles = Roles.Customer)]
        [HttpGet]
        public ActionResult EnableModules()
        {
            var model = licenseService.GetLicenseInfo(User.Identity.GetUserId<int>());
            model.ModulesInfoList = licenseService.GetModulesShortInfo(model.License, false);
            return View(model);
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public ActionResult EnableModules(List<ModuleShortInfo> model)
        {
            var licenseResult = licenseService.EditModules(User.Identity.GetUserId<int>(), model, true);
            if (licenseResult.Succed)
            {
                var license = licenseResult.GetResult();
                var modulesInfo = licenseService.GetModulesShortInfo(license, false);
                return PartialView("SwitchModules", modulesInfo);
            }
            return Content("Error");
        }


        [Authorize(Roles = Roles.Customer)]
        [HttpGet]
        public ActionResult DisableModules()
        {
            var model = licenseService.GetLicenseInfo(User.Identity.GetUserId<int>());
            model.ModulesInfoList = licenseService.GetModulesShortInfo(model.License, true);
            return View(model);
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public ActionResult DisableModules(List<ModuleShortInfo> model)
        {

            var licenseResult = licenseService.EditModules(User.Identity.GetUserId<int>(), model, false);
            if (licenseResult.Succed)
            {
                var license = licenseResult.GetResult();
                var modulesInfo = licenseService.GetModulesShortInfo(license, true);

                return PartialView("SwitchModules", modulesInfo);
            }
            return Content("Error");
        }

        //-------
        [Authorize(Roles = Roles.Customer)]
        [HttpGet]
        public ActionResult AddLicenseSeats()
        {
            var model = licenseService.GetLicenseInfo(User.Identity.GetUserId<int>());
            return View(model);
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public JsonResult AddLicenseSeats(int count)
        {
            var result = licenseService.EditLicenseSeats(User.Identity.GetUserId<int>(), count);
            return Json(result);
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpGet]
        public ActionResult RemoveLicenseSeats()
        {
            var model = licenseService.GetLicenseInfo(User.Identity.GetUserId<int>());
            return View(model);
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public JsonResult RemoveLicenseSeats(int count)
        {
            var result = licenseService.EditLicenseSeats(User.Identity.GetUserId<int>(), -count);
            return Json(result);
        }
        //-------




        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public JsonResult GetSearchInvoice(string search = "")
        {
            var result = licenseService.GetSearchedLicenseInfo(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "financeadmin")]
        [HttpGet]
        public JsonResult GetAdwenchedSearchInvoice(Models.ViewModel.SearchOption option)
        {
            var result = licenseService.GetAdwenchedSearchLicenseInfo(option);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult Modules()
        {
            return View(licenseService.GetModulesInfo());
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult ModuleInfo(int id)
        {
            return View(licenseService.GetModuleInfo(id));
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult EditModule(int id)
        {
            var module = defaultModuleRepository.GetById(id);
            var moduleLicenses = defaultLicenseRepository.GetByModuleId(module.Id);
            var allLicenses = defaultLicenseRepository.GetAll();
            EditModuleModel editModule = new EditModuleModel()
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Price = module.Price,
                ModuleLicenses = moduleLicenses.ToList(),
                AllLicenses = allLicenses.ToList()
            };
            return View(editModule);
        }

        [Authorize(Roles = "financeadmin")]
        [HttpPost]
        public ActionResult EditModule(EditModuleModel editModule, string[] selectedLicenses)
        {
            if (!ModelState.IsValid) return RedirectToAction("EditModule", new { id = editModule.Id });
            licenseService.EditModule(editModule, selectedLicenses);
            return RedirectToAction("ModuleInfo", new { id = editModule.Id });
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult CreateModule()
        {
            EditModuleModel editModule = new EditModuleModel()
            {
                ModuleLicenses = new List<DefaultLicense>(),
                AllLicenses = defaultLicenseRepository.GetAll().ToList()
            };
            return View(editModule);
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpPost]
        public ActionResult CreateModule(EditModuleModel editModule, string[] selectedLicenses)
        {
            int id = licenseService.CreateModule(editModule, selectedLicenses);
            return RedirectToAction("ModuleInfo", new { id = id });
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult Customers()
        {
            var customers = userRepository.GetCustomers();
            var companys = companyService.GetCompanysByCompanyIds(customers.Select(m => m.CompanyId).ToArray());
            var licenses = licenseRepository.GetAll();
            var usersInfo = customers.Select(m => new UserInfoViewModel()
            {
                Id = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Company = companys.Where(c => c.Id == m.CompanyId).First(),
                License = licenses.Where(l => l.Id == companys.Where(c => c.Id == m.CompanyId).First().LicenseId).First()
            });
            return View(usersInfo);
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult CustomerInfo(int id)
        {
            return View(companyService.GetUserInfo(id));
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpPost]
        public JsonResult EditCustomerModules(int licenseId, ModuleInfo[] modules, string startDate = "")
        {
            var date = DateTime.Now;
            var selectedDate = startDate.Split('.');
            if(int.Parse(selectedDate[1]) < date.Year || (int.Parse(selectedDate[1]) == date.Year && int.Parse(selectedDate[0]) < date.Month))
            {
                return Json("Invalid date");
            }
            return Json(licenseService.EditCustomerModules(licenseId, modules, startDate));
        }


        [Authorize(Roles = Roles.Customer)]
        [HttpGet]
        public FileResult NextInvoice()
        {
            var currentUser = userRepository.GetById(User.Identity.GetUserId<int>());
            var invoice = invoiceService.NextInvoice(currentUser.CompanyId);
            var file = invoiceService.GetInvoiceFile(new List<InvoiceForDownload>() { invoice });
            return File(file, "text/csv", "Invoices.csv");
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult CreateLicense()
        {
            return View();
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpPost]
        public ActionResult CreateLicense(CreateDefaultLicense model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var defaultlicense = new DefaultLicense()
            {
                Name = model.Name,
                Price = model.Price
            };
            defaultLicenseRepository.Save(defaultlicense);
            return RedirectToAction("Index", "Home");
        }
    }
}