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
            RecurringJob.AddOrUpdate(() => ChangeService.Instance.ApplyChanges(), Cron.Daily());
            RecurringJob.AddOrUpdate(() => EmailSenderService.Instance.Send(), Cron.Monthly(1));
        }
        void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }
    }
}
