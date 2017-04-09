using leavedays.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{
    public class HomeController : Controller
    {
        readonly CompanyService companyService;
        public HomeController(CompanyService companyService)
        {
            this.companyService = companyService;
        }


        public ActionResult Index(string company)
        {
            //var a = User.IsInRole();
            return View();
        }

        
       // public ActionResult Model()

    }
}