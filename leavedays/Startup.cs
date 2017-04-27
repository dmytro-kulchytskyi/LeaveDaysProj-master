using Hangfire;
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
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Server=DESKTOP-ERHGVQ5;database=NewsWebSiteDB;Integrated Security=true;");
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            ConfigureAuth(app);
        }
    }
}
