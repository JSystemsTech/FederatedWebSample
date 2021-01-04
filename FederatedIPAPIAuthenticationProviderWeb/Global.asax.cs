using FederatedIPAuthenticationService;
using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.ServiceProvider;
using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.Web.Application;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
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
