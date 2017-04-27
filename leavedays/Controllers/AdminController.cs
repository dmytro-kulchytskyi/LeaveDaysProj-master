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

        public ActionResult Send()
        {
            EmailSenderService.Send(userRepository.GetCustomers());
            return Content("ok");
        }

        [Authorize(Roles = "financeadmin")]
        [HttpGet]
        public ActionResult Invoices()
        {
            var invoices = invoiceService.GetByDeleteStatus(false);
            var result = invoices.Select(m => new InvoiceView
            {
                Id = m.Id,
                CompanyName = companyService.GetById(m.Company.Id).FullName,
                RecieveDate = m.RecieveDate
            });
            return View(result);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteInvoice(int id)
        {
            var invoice = invoiceRepository.GetById(id);
            invoice.isDeleted = true;
            invoiceRepository.Save(invoice);
            return RedirectToAction("Invoices");
        }

        [Authorize]
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
            //using (Stream stream = new MemoryStream())
            //{

            //    using (TextWriter textWriter = new StreamWriter(stream))
            //    {

            //        using (var csvWriter = new CsvWriter(textWriter))
            //        {

            //            var invoice = invoiceService.CreateInvoiceForDownload(id);
            //            csvWriter.WriteRecord(invoice);
            //        }
            //        textWriter.Flush();
            //    }
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return File(stream, "text/csv", "Invoice" + id.ToString() + ".csv");
            //}
            var stream = new MemoryStream();
            TextWriter textWriter = new StreamWriter(stream);

            CsvConfiguration csvConfiguration = new CsvConfiguration();
            csvConfiguration.Delimiter = ";";
            var map = csvConfiguration.AutoMap<InvoiceForDownload>();
            csvConfiguration.RegisterClassMap(map);

            var csvWriter = new CsvWriter(textWriter, csvConfiguration);
            var invoice = invoiceService.CreateInvoiceForDownload(id);
            csvWriter.WriteHeader(invoice.GetType());
            csvWriter.WriteRecord(invoice);
            textWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "text/csv", "Invoice" + id.ToString() + ".csv");
        }

        [Authorize]
        [HttpPost]
        public FileResult DownloadInvoices(string ids)
        {
            string[] idStringMass = ids.Split(' ');
            int[] idIntMass = new int[idStringMass.Length];
            for (int i = 0; i < idIntMass.Length; i++)
            {
                idIntMass[i] = int.Parse(idStringMass[i]);
            }
            var invoices = invoiceService.CreateInvoicesForDownload(idIntMass);
            Stream stream = new MemoryStream();
            TextWriter textWriter = new StreamWriter(stream);

            CsvConfiguration csvConfiguration = new CsvConfiguration();
            csvConfiguration.Delimiter = ";";
            var map = csvConfiguration.AutoMap<InvoiceForDownload>();
            csvConfiguration.RegisterClassMap(map);

            var csvWriter = new CsvWriter(textWriter, csvConfiguration);
            csvWriter.WriteHeader<InvoiceForDownload>();
            csvWriter.WriteRecords(invoices);
            textWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "text/csv", "Invoices.csv");
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
            model.DefaultModules = licenseService.GetDefaultModules(model.License, false);
            return View(model);
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public ActionResult EnableModules(ModuleStatus model)
        {
           
            //if (string.IsNullOrEmpty(modulesLine)) return Json(0);

            //var moduleNames = modulesLine.SplitByComma();

            //licenseService.EditModules(User.Identity.GetUserId<int>(), moduleNames, true);
            return Json(1);

        }


        [Authorize(Roles = Roles.Customer)]
        [HttpGet]
        public ActionResult DisableModules()
        {
            var model = licenseService.GetLicenseInfo(User.Identity.GetUserId<int>());
            model.DefaultModules = licenseService.GetDefaultModules(model.License, true);
            return View(model);
        }


        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public JsonResult DisableModules(string modulesLine = "")
        {
            if (string.IsNullOrEmpty(modulesLine)) return Json(0);

            var moduleNames = modulesLine.SplitByComma();

            licenseService.EditModules(User.Identity.GetUserId<int>(), moduleNames, false);
            return Json(1);
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
            var licenses = defaultLicenseRepository.GetAll();
            var modules = defaultModuleRepository.GetAll();
            var modulesInfo = modules.Select(m => new ModuleInfo()
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                Description = m.Description,
                Licenses = licenses.Where(l => l.DefaultModules.Select(k => k.Id).Contains(m.Id))
            });
            return View(modulesInfo);
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpGet]
        public ActionResult ModuleInfo(int id)
        {
            var module = defaultModuleRepository.GetById(id);
            var licenses = defaultLicenseRepository.GetByModuleId(module.Id);
            ModuleInfo moduleInfo = new ModuleInfo()
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Price = module.Price,
                Licenses = licenses
            };
            return View(moduleInfo);
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
            var module = defaultModuleRepository.GetById(editModule.Id);
            module.Name = editModule.Name;
            module.Description = editModule.Description;
            module.Price = editModule.Price;
            var selectLicenses = defaultLicenseRepository.GetByNames(selectedLicenses.ToList());
            var allLicenses = defaultLicenseRepository.GetAll();
            if (selectedLicenses != null)
            {
                foreach (var license in allLicenses)
                {
                    if (selectLicenses.Select(m => m.Id).Contains(license.Id) && !license.DefaultModules.Select(l => l.Id).Contains(module.Id))
                    {
                        license.DefaultModules.Add(module);
                    }
                    else if (!selectLicenses.Select(m => m.Id).Contains(license.Id) && license.DefaultModules.Select(l => l.Id).Contains(module.Id))
                    {
                        license.DefaultModules = new HashSet<DefaultModule>(license.DefaultModules.Where(m => m.Id != module.Id));
                    }
                }
            }
            else
            {
                foreach (var license in allLicenses)
                {
                    license.DefaultModules = new HashSet<DefaultModule>(license.DefaultModules.Where(m => m.Id != module.Id));
                }
            }
            defaultLicenseRepository.Save(allLicenses.ToList());
            defaultModuleRepository.Save(module);
            return RedirectToAction("ModuleInfo", new { id = module.Id });
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
            DefaultModule defaultModule = new DefaultModule()
            {
                Name = editModule.Name,
                Description = editModule.Description,
                Price = editModule.Price
            };
            defaultModuleRepository.Save(defaultModule);
            if (selectedLicenses != null)
            {
                var defaultLicenses = defaultLicenseRepository.GetByNames(selectedLicenses.ToList());
                foreach (var license in defaultLicenses)
                {
                    license.DefaultModules.Add(defaultModule);
                }
                defaultLicenseRepository.Save(defaultLicenses.ToList());
                var licenses = licenseRepository.GetByDefaultLicenseIds(defaultLicenses.Select(m => m.Id).ToArray());
                foreach (var license in licenses)
                {
                    Module module = new Module()
                    {
                        DefaultModuleId = defaultModule.Id,
                        Price = defaultModule.Price,
                        LicenseId = license.Id,
                        IsActive = false,
                    };
                    moduleRepository.Save(module);
                }
            }
            return RedirectToAction("ModuleInfo", new { id = defaultModule.Id });
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
            var customer = userRepository.GetById(id);
            var company = companyService.GetById(customer.CompanyId);
            var license = licenseRepository.GetById(company.LicenseId);
            UserInfoViewModel customerInfo = new UserInfoViewModel()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Company = company,
                License = license,
                Modules = moduleRepository.GetByLicenseId(license.Id).Select(m => new Models.ViewModels.License.ModuleInfo()
                {
                    Id = m.Id,
                    Name = defaultModuleRepository.GetById(m.DefaultModuleId).Name,
                    Price = m.Price,
                    isLocked = m.IsLocked
                }).ToList()
            };
            return View(customerInfo);
        }

        [Authorize(Roles = Roles.FinanceAdmin)]
        [HttpPost]
        public JsonResult EditCustomerModules(int licenseId, ModuleInfo[] modules, string startDate = "")
        {
            var defaultModules = moduleRepository.GetByLicenseId(licenseId);
            List<ModuleChange> modulesChange = new List<ModuleChange>();
            List<ModuleInfo> modulesInfo = new List<Models.ViewModels.License.ModuleInfo>();
            foreach (var defModule in defaultModules)
            {
                var res = modules.Where(m => m.Id == defModule.Id && (m.isLocked != defModule.IsLocked || m.Price != defModule.Price));
                if (res.Count() > 0)
                {
                    modulesInfo.Add(res.First());
                }
            }
            if (modulesInfo.Count > 0)
            {
                string[] date = startDate.Split('.');
                foreach (ModuleInfo module in modulesInfo)
                {
                    ModuleChange moduleChange = new ModuleChange()
                    {
                        ModuleId = module.Id,
                        IsLocked = module.isLocked,
                        Price = module.Price,
                        StartDate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]))
                    };
                    modulesChange.Add(moduleChange);
                }
                moduleChangeRepository.Save(modulesChange);
                return Json("All changes was saved");
            }
            return Json("It is nothing to change");
        }

        [Authorize]
        [HttpGet]
        public ActionResult CreateLicense()
        {
            var model = new CreateLicense()
            {
                Modules = defaultModuleRepository.GetAll()
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult CreateLicense(string modulesLine, int id, string name)
        {

            return Json("success");
        }
    }
}