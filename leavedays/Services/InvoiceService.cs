using CsvHelper;
using CsvHelper.Configuration;
using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModels.License;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IModuleChangeRepository changeRepository;
        public InvoiceService(
           IUserRepository userRepository,
           ILicenseRepository licenseRepository,
           ICompanyRepository companyRepository,
           IInvoiceRepository invoiceRepository,
           IModuleRepository moduleRepository,
           IModuleChangeRepository changeRepository)
        {
            this.changeRepository = changeRepository;
            this.userRepository = userRepository;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
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
            var modules = moduleRepository.GetByLicenseId(license.Id).Where(m => m.IsActive).ToList();
            var invoiceForDownload = new InvoiceForDownload
            {
                Id = invoice.Id,
                LicenceCode = license.LicenseCode,
                CompanyName = company.FullName,
                ContactPerson = owner.LastName + " " + owner.FirstName,
                TotalPrice = (modules.Where(m => !m.IsLocked).Sum(m => m.Price) * companyRepository.GetUsersCount(company.Id)),
                SeatsNumber = license.Seats,
                Modules = moduleRepository.GetForDownload(modules.Select(m => m.Id).ToArray(), false)
            };
            return invoiceForDownload;
        }
        public List<InvoiceForDownload> CreateInvoicesForDownload(int[] ids)
        {
            List<InvoiceForDownload> invoices = new List<InvoiceForDownload>();
            foreach (int id in ids)
            {
                invoices.Add(CreateInvoiceForDownload(id));
            }
            return invoices;

        }

        public byte[] GetInvoiceFile(List<InvoiceForDownload> invoices)
        {
            using (var stream = new MemoryStream())
            using (TextWriter textWriter = new StreamWriter(stream))
            {
                CsvConfiguration configuration = new CsvConfiguration()
                {
                    Delimiter = ";"
                };
                configuration.RegisterClassMap(configuration.AutoMap<InvoiceForDownload>());
                using (var csvWriter = new CsvWriter(textWriter, configuration))
                {
                    csvWriter.WriteHeader<InvoiceForDownload>();
                    foreach(var invoice in invoices)
                    {
                        csvWriter.WriteRecord(invoice);
                        csvWriter.WriteRecords(invoice.Modules);
                    }
                    textWriter.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream.ToArray();
                }
            }
        }

        public InvoiceForDownload NextInvoice(int companyId)
        {
            var currentDate = DateTime.Now;
            var company = companyRepository.GetById(companyId);
            var owner = userRepository.GetOwnerByCompanyId(company.Id);
            var license = licenseRepository.GetById(company.LicenseId);
            var modules = moduleRepository.GetByLicenseId(license.Id).Where(m => m.IsActive).ToList();
            List<ModuleForDownload> modulesForDownload = new List<ModuleForDownload>(modules.Count);
            foreach(var module in modules)
            {
                var changeModule = changeRepository.GetByDate(currentDate.Year, currentDate.Month + 1, module.Id);
                if(changeModule.Count != 0)
                {
                    if (!changeModule.First().IsLocked)
                    {
                        var moduleForDownload = moduleRepository.GetForDownload(new[] { changeModule.First().ModuleId }, true).First();
                        moduleForDownload.Price = changeModule.First().Price;
                        modulesForDownload.Add(moduleForDownload);
                    }
                }
                else
                {
                    if(!module.IsLocked)
                        modulesForDownload.Add(moduleRepository.GetForDownload(new[] { module.Id }, false).First());
                }
            }
            var invoiceForDownload = new InvoiceForDownload
            {
                Id = 1,
                LicenceCode = license.LicenseCode,
                CompanyName = company.FullName,
                ContactPerson = owner.LastName + " " + owner.FirstName,
                TotalPrice = (modulesForDownload.Sum(m => m.Price) * companyRepository.GetUsersCount(companyId)),
                SeatsNumber = license.Seats,
                Modules = modulesForDownload
            };
            return invoiceForDownload;
        }
    }
}