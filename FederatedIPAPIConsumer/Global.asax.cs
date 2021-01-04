using FederatedIPAuthenticationService.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FederatedIPAuthenticationService.Web.Application;
using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.ServiceProvider;
using ServiceLayer.Services;

namespace FederatedIPAPIConsumer
{


    public class MvcApplication : MvcServiceApplication
    {        
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationSettings();
            services.AddConnectionStringConfig();
            services.ConfigureAsFederatedConsumer();
            services.AddTokenProvider();
            services.AddService<IUserManagmentService, UserManagmentService>();
        }
        //public override void Init()
        //{
        //    base.Init();
        //}
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
