using FederatedAuthNAuthZ.Web.ConsumerAPI;
using FederatedIPAuthenticationService.Web.TokenProviderAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;

namespace FederatedIPAuthenticationService.Extensions
{
    public static class HttpConfigurationExtensions
    {
        public static void ConfigureTokenProviderAPI(this HttpConfiguration config, string controllerName)
        {
            config.Routes.MapHttpRoute(
                name: "TokenProviderAPI",
                routeTemplate: "TokenProviderAPI/{action}/{id}",
                defaults: new { controller = controllerName, id = RouteParameter.Optional }
            );
        }
        public static void ConfigureTokenProviderAPI<TTokenProviderAPIController>(this HttpConfiguration config)
            where TTokenProviderAPIController : TokenProviderAPIControllerBase
        {
            config.ConfigureTokenProviderAPI(typeof(TTokenProviderAPIController).Name.Replace("Controller", ""));
        }
        public static string GetTokenProviderAPIEndpointName(this string action) => $"TokenProviderAPI/{action}";
        public static void ConfigureApplicationAuthenticationAPI(this HttpConfiguration config, string controllerName)
        {
            config.Routes.MapHttpRoute(
                name: "ApplicationAuthenticationAPI",
                routeTemplate: "ApplicationAuthenticationAPI/{action}/{id}",
                defaults: new { controller = controllerName, id = RouteParameter.Optional }
            );
        }
        public static void ConfigureApplicationAuthenticationAPI<TApplicationAuthenticationAPIController>(this HttpConfiguration config)
            where TApplicationAuthenticationAPIController : ApplicationAuthenticationAPIControllerBase
        {
            config.ConfigureApplicationAuthenticationAPI(typeof(TApplicationAuthenticationAPIController).Name.Replace("Controller", ""));
        }
        public static string GetApplicationAuthenticationAPIEndpointName(this string action) => $"ApplicationAuthenticationAPI/{action}";
    }
}
