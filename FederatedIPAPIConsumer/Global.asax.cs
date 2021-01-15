using FederatedIPAuthenticationService.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FederatedIPAuthenticationService.Services;
using ServiceLayer.Services;
using ServiceLayer.DomainLayer.DbConnection;
using ServiceLayer.DomainLayer;
using ServiceProvider.Web;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Services;

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
            services.AddService<DbConnectionConfigService>();
            services.AddService<IDomainFacade, DomainFacade>();
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
