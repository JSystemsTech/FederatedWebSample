using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace FederatedAuthNAuthZ.Extensions
{
    internal static class HttpContextExtensions
    {
        public static string GetSessionValue(this HttpContextBase httpContext, string key)
        {
            if(httpContext.Session[key] != null)
            {
                return httpContext.Session[key].ToString();
            }
            return null;
        }
        public static void SetSessionValue(this HttpContextBase httpContext, string key, object value)
        {
            httpContext.Session.Remove(key);
            httpContext.Session.Add(key, value);
        }
        private static string AuthenticationRequestTokenCookieSuffix => "AuthenticationRequestTokenCookieSuffix";
        public static string GetAuthenticationRequestTokenCookieSuffix(this HttpContextBase httpContext) 
            => httpContext.GetSessionValue(AuthenticationRequestTokenCookieSuffix);
        public static void SetAuthenticationRequestTokenCookieSuffix(this HttpContextBase httpContext, string value)
            => httpContext.SetSessionValue(AuthenticationRequestTokenCookieSuffix, value);

        public static bool HasAuthenticationRequestTokenCookieSuffix(this HttpContextBase httpContext)
            => httpContext.GetAuthenticationRequestTokenCookieSuffix() != null;
    }
}
