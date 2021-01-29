using FederatedIPAPIAuthenticationProviderWeb.App_Start;
using FederatedIPAPIAuthenticationProviderWeb.Configuration;
using FederatedIPAPIAuthenticationProviderWeb.Services;
using FederatedAuthNAuthZ.Services;
using ServiceLayer.DomainLayer;
using ServiceLayer.DomainLayer.DbConnection;
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
            services.ConfigureFederatedApplication<SelfContainedTokenProvider>();
            services.AddService<AuthenticationProviderDbConnectionConfigService>();
            services.AddService<IAuthenticationProviderDomainFacade, AuthenticationProviderDomainFacade>();
            services.AddAPIAuthenticationService<TokenProviderAPIAuthenticationService>();            
            services.AddTokenHandlerService<Saml2TokenHandlerService>();
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
