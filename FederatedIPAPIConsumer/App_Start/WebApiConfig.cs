using FederatedIPAPIConsumer.Controllers.AuthenticationApi;
using FederatedIPAuthenticationService.Extensions;
using System.Web.Http;

namespace FederatedIPAPIConsumer.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.ConfigureApplicationAuthenticationAPI<ApplicationAuthenticationAPIController>();
        }
    }
}