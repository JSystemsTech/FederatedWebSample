using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FederatedIPAuthenticationService.Attributes
{
    public abstract class FederatedAuthenticationProvider : FederatedAuthenticationFilter
    {

        protected IServices Services { get => HttpContext.ApplicationInstance is IMvcServiceApplication app ? app.Services : null; }
        private IFederatedProviderSettings FederatedProviderSettings => Services.Get<IFederatedProviderSettings>();
        
        private ITokenProvider TokenProvider => Services.Get<ITokenProvider>();
        private ISiteMeta SiteMeta => Services.Get<ISiteMeta>();
        protected IConsumerAuthenticationApi ConsumerAPI { get; private set; }
        private string AuthenticationRequestCookieName { get => $"{FederatedSettings.FederatedAuthenticationRequestTokenCookiePrefix}{SiteMeta.SiteId}"; }

        private string AuthenticationRequestKey => Request.Form["AuthenticationRequest"];
        private bool IsExternalAuthenticationPostbackRequest
        {
            get => AuthenticationContext.ActionDescriptor.GetCustomAttributes(typeof(FederatedExternalPostbackAttribute), true).Length>0  &&
                IsPostRequest &&
                AuthenticationRequestKey is string postToken &&
                !string.IsNullOrWhiteSpace(postToken);
        }
        protected abstract string GetSavedAuthenticationRequest(string key);

        public FederatedAuthenticationProvider(bool isAuthenticatedRoute = true) : base(isAuthenticatedRoute) { }
        private void OnAuthenticationError(AuthenticationContext filterContext, string error, string details)
        {
            if (!string.IsNullOrWhiteSpace(FederatedProviderSettings.DefaultConsumerUrl))
            {
                filterContext.Result = new RedirectResult(GetUri(FederatedProviderSettings.DefaultConsumerUrl).AbsoluteUri);
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult(error);
                HttpContext.Session.Add("HttpUnauthorizedResultDetails", details);
            }
        }
        private string RenewAuthenticationRequestToken(string token)
        {
            string authenticationRequestToken = TokenProvider.RenewToken(token, claims => {
                claims.AddUpdate("AuthenticationProviderId", "FederatedIPAPIAuthenticationProviderWeb");
            });
            HttpContext.Session.Add("AuthenticationRequestToken", authenticationRequestToken);
            HttpContext.Session.Add("ConsumerAuthenticationApiUrl", TokenProvider.GetTokenClaims(authenticationRequestToken)["ConsumerAuthenticationApiUrl"].FirstOrDefault());
            
            SetAuthenticationRequestTokenCookie(AuthenticationRequestCookieName, authenticationRequestToken, (DateTime)TokenProvider.GetExpirationDate(authenticationRequestToken));
            return authenticationRequestToken;
        }
        /* Process Request 
         * 1.If Request is a logout request handle that request
         * 2. If Request is tagged as an Authenticated Route then check for valid Authentication credendials
         * 3. Else Assume Request is to a url that does not require authentication to process.
         */
        public sealed override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
            if (IsAuthenticatedRoute)
            {
                if (IsExternalAuthenticationPostbackRequest)
                {
                    if (string.IsNullOrWhiteSpace(AuthenticationRequestKey))
                    {
                        OnAuthenticationError(filterContext, "Postback missing AuthenticationRequest form value", "External Authentication failed to return the required AuthenticationRequest form field");
                        return;
                    }
                    try
                    {
                        string authenticationRequestToken = GetSavedAuthenticationRequest(AuthenticationRequestKey);
                        if (!TokenProvider.IsValidToken(authenticationRequestToken))
                        {
                            OnAuthenticationError(filterContext, "Invalid Authentication Request Token", "Token External Authentication Pastback is not valid");
                            return;
                        }
                        RenewAuthenticationRequestToken(authenticationRequestToken);
                    }
                    catch (Exception e)
                    {
                        OnAuthenticationError(filterContext, "Invalid Authentication Request Token error " + e.Message, "Token External Authentication Pastback is not valid");
                    }
                }
                else
                {
                    string token = GetAuthenticationRequestCookieValue();
                    if (!TokenProvider.IsValidToken(token) && !IsExternalAuthenticationPostbackRequest)
                    {
                        OnAuthenticationError(filterContext, "Invalid Authentication Request Token", "Authentication Request Cookie does not contain a valid token");
                        return;
                    }
                    RenewAuthenticationRequestToken(token);
                }              
                
            }            
        }


        private string BuildUrl<T>(Uri uri, T query)
        {
            UriBuilder builder = new UriBuilder(uri);
            List<string> queryParams = new List<string>();
            (builder.Query.StartsWith("?") ? builder.Query.Remove(0, 1) : builder.Query).Split('&').Where(value=> !string.IsNullOrWhiteSpace(value)).ToList().ForEach(p =>
            {
                queryParams.Add(p);
            });
            foreach (var prop in query.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                queryParams.Add($"{prop.Name}={HttpUtility.UrlEncode(prop.GetValue(query).ToString())}");
            }
            builder.Query = string.Join("&", queryParams);
            return builder.Uri.ToString();
        }
        private string GetAuthenticationRequestCookieValue() => GetAuthenticationRequestCookie() is HttpCookie cookie ? cookie.Value : null;
        private HttpCookie GetAuthenticationRequestCookie() => HasValidHttpCookie(AuthenticationRequestCookieName) ? GetHttpCookie(AuthenticationRequestCookieName) : null;
        private void RemoveAuthenticationRequestCookie() => RemoveHttpCookie(GetAuthenticationRequestCookie(), true);
        

    }
}
