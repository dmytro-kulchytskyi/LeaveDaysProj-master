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
        private readonly LicenseService licenseService;
        private readonly IDefaultModuleRepository defaultModuleRepository;
        private readonly IDefaultLicenseRepository defaultLicenseRepository;


        public AdminController(
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
            Stream stream = new MemoryStream();
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

        [Authorize(Roles = "financeadmin")]
        [HttpGet]
        public ActionResult LicensesInfo()
        {
            return View("ShowLicensesInfo", licenseService.GetLicenseInfoList());
        }

        [Authorize(Roles = "customer")]
        [HttpGet]
        public ActionResult EnableModules()
        {
            var user = userRepository.GetById(User.Identity.GetUserId<int>());
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

        [Authorize(Roles = "customer")]
        [HttpPost]
        public JsonResult EnableModules(string modulesLine = "")
        {
            if (string.IsNullOrEmpty(modulesLine)) return Json("error");

            var moduleNames = companyService.SplitLine(modulesLine);

            if (moduleNames.Length == 0) return Json("error");

            var user = userRepository.GetById(User.Identity.GetUserId<int>());
            var companyId = user.CompanyId;
            var company = companyRepository.GetById(companyId);
            var licenseId = company.LicenseId;
            var modules = moduleRepository.GetByLicenseId(licenseId);

            modules = modules.Where(module => moduleNames.Contains(defaultModuleRepository.GetById(module.DefaultModuleId).Name)).ToList();
            foreach (var m in modules)
            {
                m.IsActive = true;
                moduleRepository.Save(m);
            }
            return Json("success");
        }


        [Authorize(Roles = "customer")]
        [HttpGet]
        public ActionResult DisableModules()
        {
            var user = userRepository.GetById(User.Identity.GetUserId<int>());
            var companyId = user.CompanyId;
            var company = companyRepository.GetById(companyId);
            var licenseId = company.LicenseId;
            var license = licenseRepository.GetById(licenseId);
            var disabledModules = moduleRepository.GetByLicenseId(licenseId, true);
            var defaultModules = disabledModules.Select(module => defaultModuleRepository.GetById(module.Id)).ToList();
            var model = new EditLicenseModules()
            {
                LicenseName = defaultLicenseRepository.GetById(license.DefaultLicenseId).Name,
                LicenseCode = license.LicenseCode,
                Modules = defaultModules
            };


            return View(model);
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        public JsonResult DisableModules(string modulesLine = "")
        {
            if (string.IsNullOrEmpty(modulesLine)) return Json("error");

            var moduleNames = companyService.SplitLine(modulesLine);

            if (moduleNames.Length == 0) return Json("error");

            var user = userRepository.GetById(User.Identity.GetUserId<int>());
            var companyId = user.CompanyId;
            var company = companyRepository.GetById(companyId);
            var licenseId = company.LicenseId;
            var modules = moduleRepository.GetByLicenseId(licenseId);

            modules = modules.Where(module => moduleNames.Contains(defaultModuleRepository.GetById(module.DefaultModuleId).Name)).ToList();
            foreach (var m in modules)
            {
                m.IsActive = false;
                moduleRepository.Save(m);
            }
            return Json("success");
        }



        [Authorize(Roles = "financeadmin")]
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