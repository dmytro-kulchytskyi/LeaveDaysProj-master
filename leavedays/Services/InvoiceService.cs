using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModels.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace leavedays.Services
{
    public class InvoiceService
    {
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IModuleRepository moduleRepository;
        public InvoiceService(
           IUserRepository userRepository,
           ILicenseRepository licenseRepository,
           ICompanyRepository companyRepository,
           IInvoiceRepository invoiceRepository,
           IModuleRepository moduleRepository)
        {
            this.userRepository = userRepository;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
        }
        public List<Invoice> GetByDeleteStatus(bool status)
        {
            return invoiceRepository.GetByDeleteStatus(status).ToList();
        }

        public List<Invoice> GetInvoices()
        {
            return invoiceRepository.GetByDeleteStatus(false).ToList();
        }

        public Invoice GetById(int id)
        {
            return invoiceRepository.GetById(id);
        }

        public List<Invoice> GetByIds(int[] ids)
        {
            return invoiceRepository.GetByIds(ids).ToList();
        }

        public void DeleteInvoices(List<Invoice> invoices)
        {
            for (int i = 0; i < invoices.Count; i++)
            {
                invoices[i].isDeleted = true;
            }
            invoiceRepository.Save(invoices);
        }

        public void Save(Invoice invoice)
        {
            invoiceRepository.Save(invoice);
        }
        public void Save(List<Invoice> invoices)
        {
            invoiceRepository.Save(invoices);
        }

        public InvoiceForDownload CreateInvoiceForDownload(int id)
        {
            var invoice = invoiceRepository.GetById(id);
            var company = companyRepository.GetById(invoice.Company.Id);
            var owner = userRepository.GetOwnerByCompanyId(company.Id);
            var license = licenseRepository.GetById(company.LicenseId);
            var modules = moduleRepository.GetByLicenseId(license.Id);
            var invoiceForDownload = new InvoiceForDownload
            {
                Id = invoice.Id,
                LicenceCode = license.LicenseCode,
                CompanyName = company.FullName,
                ContactPerson = owner.LastName + " " + owner.FirstName,
                Modules = modules.ToList()
            };
            return invoiceForDownload;
        }
        public List<InvoiceForDownload> CreateInvoicesForDownload(int[] ids)
        {
            List<InvoiceForDownload> invoices = new List<InvoiceForDownload>();
            foreach(int id in ids)
            {
                invoices.Add(CreateInvoiceForDownload(id));
            }
            return invoices;

        }
        //public byte[] GetInvoiceBytes(InvoiceForDownload invoiceForDownload)
        //{
        //    string invoiceText = "";
        //    invoiceText += "InvoiceId;" + invoiceForDownload.Id.ToString() + Environment.NewLine;
        //    invoiceText += "CompanyName;" + invoiceForDownload.Company.FullName + Environment.NewLine;
        //    invoiceText += "Contact Person;" + invoiceForDownload.Owner.FirstName + " " + invoiceForDownload.Owner.LastName + Environment.NewLine;
        //    invoiceText += "LicenseId;" + invoiceForDownload.License.Id.ToString() + Environment.NewLine;
        //    invoiceText += "Modules";
        //    var modules = moduleRepository.GetByLicenseId(invoiceForDownload.License.Id);
        //    foreach (var module in modules)
        //    {
        //        invoiceText += ";" + module.Id.ToString();

        //    }
        //    invoiceText += Environment.NewLine;
        //    invoiceText += "ModulesPrice";
        //    foreach (var module in modules)
        //    {
        //        invoiceText += ";" + module.Price.ToString();
        //    }
        //    invoiceText += Environment.NewLine;
        //    invoiceText += Environment.NewLine;
        //    byte[] result = Encoding.Default.GetBytes(invoiceText);
        //    return result;
        //}
        //public byte[] GetInvoicesBytes(List<InvoiceForDownload> invoices)
        //{
        //    List<byte> invoicesBytes = new List<byte>();
        //    foreach(var invoice in invoices)
        //    {
        //        invoicesBytes.AddRange(GetInvoiceBytes(invoice).ToList());
        //    }
        //    return invoicesBytes.ToArray();
        //}
    }
}