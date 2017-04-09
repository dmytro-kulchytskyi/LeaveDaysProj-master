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
        }
    }
}
