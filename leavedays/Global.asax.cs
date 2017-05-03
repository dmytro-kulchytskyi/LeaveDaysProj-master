using Hangfire;
using Hangfire.SqlServer;
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
            JobStorage.Current = new SqlServerStorage(System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
            RecurringJob.AddOrUpdate(() => ChangeService.Instance.ApplyChanges(), Cron.Monthly(1));
            RecurringJob.AddOrUpdate(() => EmailSenderService.Instance.Send(), Cron.Monthly(1));
            RecurringJob.AddOrUpdate(() => ChangeService.Instance.LockLicense(), Cron.Monthly(5));
        }
        void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }
    }
}
