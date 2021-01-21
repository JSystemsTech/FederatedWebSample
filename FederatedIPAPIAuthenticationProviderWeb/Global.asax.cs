using FederatedIPAPIAuthenticationProviderWeb.App_Start;
using FederatedIPAPIAuthenticationProviderWeb.Configuration;
using FederatedIPAPIAuthenticationProviderWeb.Services;
using FederatedIPAuthenticationService.Services;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Services;
using ServiceProvider.Web;
using System.Web.Http;
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
            services.AddConfiguration<ITokenProviderServiceSettings, TokenProviderServiceSettings>();

            services.ConfigureAsFederatedProvider();
            services.AddAuthenticationRequestCache();
            //services.AddTokenProvider();
            services.AddEncryptionService();
            services.AddMailService();
            services.AddService<ITokenProviderService, Saml2TokenProviderService>();
            services.AddService<ITokenProvider, SelfContainedTokenProvider>();
            
        }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(TokenProviderWebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
