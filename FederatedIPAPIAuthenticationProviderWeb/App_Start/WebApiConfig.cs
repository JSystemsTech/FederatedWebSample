using FederatedIPAPIAuthenticationProviderWeb.Controllers;
using FederatedIPAuthenticationService.Extensions;
using System.Web.Http;

namespace FederatedIPAPIAuthenticationProviderWeb.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.ConfigureTokenProviderAPI<TokenProviderAPIController>();
        }
    }
}