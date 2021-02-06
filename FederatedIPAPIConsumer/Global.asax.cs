using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using FederatedIPAPIConsumer.App_Start;
using FederatedIPAPIConsumer.Configuration;
using FederatedIPAPIConsumer.Services;
using ServiceLayer.DomainLayer;
using ServiceLayer.DomainLayer.DbConnection;
using ServiceLayer.Services;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Services;
using ServiceProvider.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FederatedIPAPIConsumer
{


    public class MvcApplication : MvcServiceApplication
    {        
        public override void ConfigureServices(IServiceCollection services)
        {
            
            services.AddApplicationSettings();
            services.AddConnectionStringConfig();
            services.AddConfiguration<IDbStoredApplicationSettingsConfig, DbStoredApplicationSettingsConfig>();

            services.ConfigureFederatedApplication<ConsumingApplicationSettings, FederatedEncryptionServiceWithDateSalt>();
            services.AddFederatedApplicationIdentityService<ConsumingApplicationIdentityService>();
            services.AddService<DbConnectionConfigService>();
            services.AddService<IDomainFacade, DomainFacade>();
            services.AddService<IUserManagmentService, UserManagmentService>();
            services.AddService<ICssThemeService, CssThemeService>();

        }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
