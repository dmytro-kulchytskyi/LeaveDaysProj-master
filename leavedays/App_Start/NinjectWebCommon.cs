[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(leavedays.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(leavedays.App_Start.NinjectWebCommon), "Stop")]

namespace leavedays.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using Models;
    using Services;
    using Models.Repository.Interfaces;
    using Models.Repository;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Models.Identity;
    using Microsoft.Owin.Security;

    public static class NinjectWebCommon
    {
        public static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<InvoiceService>().To<InvoiceService>();
            kernel.Bind<CompanyService>().To<CompanyService>();
            kernel.Bind<RequestService>().To<RequestService>();
            kernel.Bind<LicenseService>().To<LicenseService>();
            kernel.Bind<ChangeService>().To<ChangeService>();
            kernel.Bind<EmailSenderService>().To<EmailSenderService>();

            kernel.Bind<IUserRepository>().To<UserRepository>();
            kernel.Bind<ICompanyRepository>().To<CompanyRepository>();

            kernel.Bind<IRequestRepository>().To<RequestRepository>();
            kernel.Bind<IRoleRepository>().To<RoleRepository>();

            kernel.Bind<ILicenseRepository>().To<LicenseRepository>();
            kernel.Bind<IModuleRepository>().To<ModuleRepository>();

            kernel.Bind<IModuleChangeRepository>().To<ModuleChangeRepository>();

            kernel.Bind<IDefaultLicenseRepository>().To<DefaultLicenseRepository>();
            kernel.Bind<IDefaultModuleRepository>().To<DefaultModuleRepository>();
            kernel.Bind<IInvoiceRepository>().To<InvoiceRepository>();

            kernel.Bind<UserManager<AppUser, int>>().To<UserManager<AppUser, int>>();

            kernel.Bind<SignInManager<AppUser, int>>().To<SignInManager<AppUser, int>>();
            kernel.Bind<RoleManager<Role, int>>().To<RoleManager<Role, int>>();

            kernel.Bind<IRoleStore<Role, int>>().To<CustomRoleStore>();
            kernel.Bind<IUserStore<AppUser, int>>().To<CustomUserStore>();

            kernel.Bind<IAuthenticationManager>().ToMethod(_ => HttpContext.Current.GetOwinContext().Authentication);

            kernel.Bind<ISessionFactory>().ToMethod(context =>
            {
                var configuration = new Configuration();

                configuration.Configure();
                configuration.AddAssembly(typeof(AppUser).Assembly);
                configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
                ISessionFactory sessionFactory = configuration.BuildSessionFactory();
                new SchemaUpdate(configuration).Execute(true, true);
                return sessionFactory;
            }).InSingletonScope();
        }
    }
}
