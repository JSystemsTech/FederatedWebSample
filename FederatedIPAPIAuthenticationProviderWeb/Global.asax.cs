using FederatedIPAuthenticationService.Services;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Services;
using ServiceProvider.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FederatedIPAPIAuthenticationProviderWeb
{
    public class MvcApplication : MvcServiceApplication
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationSettings();
            services.AddConnectionStringConfig();
            services.ConfigureAsFederatedProvider();
            services.AddAuthenticationRequestCache();
            services.AddTokenProvider();
            services.AddEncryptionService();
            services.AddMailService();
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
