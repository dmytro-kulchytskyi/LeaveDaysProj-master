using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;
using CsvHelper.Configuration;

namespace leavedays.Models.ViewModels.License
{
    public class InvoiceForDownload
    {
        public int Id { get; set; }
        public string LicenceCode { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public int SeatsNumber { get; set; }
        public double TotalPrice { get; set; }
        public IEnumerable<ModuleForDownload> Modules { get; set; }
    }

    public sealed class InvoiceForDownloadCsvMap:CsvClassMap<InvoiceForDownload>
    {
        public InvoiceForDownloadCsvMap()
        {
            Map(m => m.Id);
            Map(m => m.LicenceCode);
            Map(m => m.CompanyName);
            Map(m => m.ContactPerson);
        }
    }
}