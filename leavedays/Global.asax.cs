using Hangfire;
using leavedays.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace leavedays
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //RecurringJob.AddOrUpdate(() => ChangeService.ApplyChanges(), Cron.Daily());

        }
        void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }
    }
}
