using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Extensions;
using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Web;
using ServiceProviderShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FederatedAuthNAuthZ.Attributes
{
    public abstract class FederatedAuthenticationProvider : FederatedAuthenticationFilter
    {
        
        private ITokenProvider TokenProvider => ServiceManager.GetService<ITokenProvider>();
        private IEncryptionService EncryptionService => ServiceManager.GetService<IEncryptionService>();
        private IFederatedApplicationSettings FederatedApplicationSettings => ServiceManager.GetService<IFederatedApplicationSettings>();
        private string AuthenticationRequestCookieName { get => $"{FederatedApplicationSettings.GetAuthRequestCookieName()}{AuthenticationRequestTokenCookieSuffix}"; }
        private string AuthenticationRequestTokenCookieSuffix => HttpContext.Session["AuthenticationRequestTokenCookieSuffix"].ToString();
        private string AuthenticationRequestKey => Request.Form["AuthenticationRequest"];
        private string  ConsumerAuthenticationApiUrl { get; set; }
        private bool IsExternalAuthenticationPostbackRequest
        {
            get => AuthenticationContext.ActionDescriptor.GetCustomAttributes(typeof(FederatedExternalPostbackAttribute), true).Length>0  &&
                IsPostRequest &&
                AuthenticationRequestKey is string postToken &&
                !string.IsNullOrWhiteSpace(postToken);
        }
        protected abstract string GetSavedConsumerAuthenticationApiUrl(string key);

        public FederatedAuthenticationProvider(bool isAuthenticatedRoute = true) : base(isAuthenticatedRoute) { }
        private void OnAuthenticationError(AuthenticationContext filterContext, string error, string details)
        {
            if (!string.IsNullOrWhiteSpace(FederatedApplicationSettings.AuthenticationErrorUrl))
            {
                filterContext.Result = new RedirectResult(GetUri(FederatedApplicationSettings.AuthenticationErrorUrl).AbsoluteUri);
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult(error);
                HttpContext.Session.Add("HttpUnauthorizedResultDetails", details);
            }
        }
        private string RenewAuthenticationRequestToken(string token)
        {
            string authenticationRequestToken = TokenProvider.RenewToken(token, claims => { });
            ConsumerAuthenticationApiUrl = TokenProvider.GetTokenClaims(authenticationRequestToken)["ConsumerAuthenticationApiUrl"].FirstOrDefault();
            SetAuthenticationRequestTokenCookie(AuthenticationRequestCookieName, authenticationRequestToken, (DateTime)TokenProvider.GetExpirationDate(authenticationRequestToken));
            return authenticationRequestToken;
        }
        private void ValidateRequestingSite(AuthenticationContext filterContext)
        {
            IApplicationAuthenticationAPI ApplicationAuthenticationAPI = new ApplicationAuthenticationAPI(ConsumerAuthenticationApiUrl);
            if (!ApplicationAuthenticationAPI.GetApplicationSettings().FederatedApplicationSettings.IsSameNetwork(FederatedApplicationSettings))
            {
                OnAuthenticationError(filterContext, "Invalid Authentication Request", "Requesting Site is not on the same network");
            }
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
                        string authenticationRequestToken =  TokenProvider.CreateToken(claims => {
                            claims.Add("ConsumerAuthenticationApiUrl", new string[] { GetSavedConsumerAuthenticationApiUrl(AuthenticationRequestKey) });
                        });

                        HttpContext.Session.Remove("AuthenticationRequestTokenCookieSuffix");
                        HttpContext.Session.Add("AuthenticationRequestTokenCookieSuffix", CreateAuthenticationRequestTokenCookieSuffix());
                        RenewAuthenticationRequestToken(authenticationRequestToken);
                        ValidateRequestingSite(filterContext);
                    }
                    catch (Exception e)
                    {
                        OnAuthenticationError(filterContext, "Invalid Authentication Request Token error " + e.Message, "Token External Authentication Pastback is not valid");
                    }
                }
                else
                {
                    if(Request.QueryString["LoginRequestToken"] is string loginRequestToken)
                    {
                        if(EncryptionService.DateSaltDecrypt(loginRequestToken, true) is string authenticationRequestTokenCookieSuffix)
                        {
                            HttpContext.Session.Remove("AuthenticationRequestTokenCookieSuffix");
                            HttpContext.Session.Add("AuthenticationRequestTokenCookieSuffix", authenticationRequestTokenCookieSuffix);
                        }
                        else
                        {
                            OnAuthenticationError(filterContext, "Invalid Authentication Request Token", "Authentication Request Cookie does not contain a valid token");
                            return;
                        }                       
                        
                    }

                    string token = GetAuthenticationRequestCookieValue();
                    if (!TokenProvider.IsValidToken(token))
                    {
                        OnAuthenticationError(filterContext, "Invalid Authentication Request Token", "Authentication Request Cookie does not contain a valid token");
                        return;
                    }
                    RenewAuthenticationRequestToken(token);
                    ValidateRequestingSite(filterContext);
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
