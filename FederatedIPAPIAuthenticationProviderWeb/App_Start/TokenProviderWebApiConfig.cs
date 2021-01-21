using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace FederatedIPAPIAuthenticationProviderWeb.App_Start
{
    public static class TokenProviderWebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}