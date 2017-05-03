﻿using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(leavedays.Startup))]
namespace leavedays
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"server=dimas;database = test;integrated security = true;");
            app.UseHangfireServer();
        }
    }
}
