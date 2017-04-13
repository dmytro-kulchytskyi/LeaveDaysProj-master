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


        public AdminController(
           CompanyService companyService,
           IUserRepository userRepository,
           LicenseService licenseService,
           ILicenseRepository licenseRepository,
           ICompanyRepository companyRepository,
           IInvoiceRepository invoiceRepository,
           IModuleRepository moduleRepository,
           InvoiceService invoiceService)
        {
            this.licenseService = licenseService;
            this.userRepository = userRepository;
            this.companyService = companyService;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
            this.invoiceService = invoiceService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Invoices()
        {
            var invoices = invoiceRepository.GetByDeleteStatus(false);
            var result = invoices.Select(m => new InvoiceView
            {
                Id = m.Id,
                CompanyName = companyRepository.GetById(m.Company.Id).FullName,
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
            var invoice = invoiceService.CreateInvoiceForDownload(id);
            byte[] invoiceBytes = invoiceService.GetInvoiceBytes(invoice);
            return File(invoiceBytes, "text/csv", "Invoice" + invoice.Id.ToString() + ".csv");
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
            byte[] invoicesBytes = invoiceService.GetInvoicesBytes(invoices);
            return File(invoicesBytes, "text/csv", "Invoices.csv");

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

        [Authorize]
        [HttpGet]
        public ActionResult ShowLicensesInfo()
        {
            return View(licenseService.GetLicenseInfoList());
        }
        

        [Authorize]
        [HttpGet]
        public ActionResult GetSearchInvoice(string search = "")
        {
            var result = licenseService.GetLicenseInfoList();
            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(m => m.CompanyName.Contains(search) || m.ContactPerson.Contains(search) || m.Email.Contains(search) || m.LicenceCode.Contains(search) || m.PhoneNumber.Contains(search)).ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}