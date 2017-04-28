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
            // GlobalConfiguration.Configuration.UseSqlServerStorage(@"Server=tcp:leavedays-db.database.windows.net,1433;Initial Catalog=leavedays;Persist Security Info=False;User ID=team3;Password=Hozzy1337;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;");
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Server=DIMAS;database=test;Integrated Security=true;");
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            ConfigureAuth(app);
        }
    }
}
