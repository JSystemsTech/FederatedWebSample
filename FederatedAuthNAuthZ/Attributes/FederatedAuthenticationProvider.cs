using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Extensions;
using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using ServiceProviderShared;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FederatedAuthNAuthZ.Attributes
{
    public sealed class FederatedAuthenticationProvider : FederatedAuthenticationFilter
    {
        
        
        private ITokenProvider TokenProvider => ServiceManager.GetService<ITokenProvider>();
        private IFederatedApplicationSettings FederatedApplicationSettings => ServiceManager.GetService<IFederatedApplicationSettings>();
        private string AuthenticationRequestCookieName => $"{FederatedApplicationSettings.GetAuthRequestCookieName()}{HttpContext.GetAuthenticationRequestTokenCookieSuffix()}"; 
        private string ConsumerAuthenticationApiFormParam => Request.Form["api"];
        private string  ConsumerAuthenticationApiUrl { get; set; }
        private bool IsPostback{ get; set; }
        private bool IsValidPostback => IsPostback && ConsumerAuthenticationApiFormParam is string apiKey && !string.IsNullOrWhiteSpace(apiKey);
        private string AuthenticationRequestFormParameter { get; set; }
        public FederatedAuthenticationProvider(bool isAuthenticatedRoute = true, bool isPostback = false) : base(isAuthenticatedRoute) {
            IsPostback = isPostback;
        }
        
        private void OnAuthenticationError(AuthenticationContext filterContext, string error, string details)
        {
            if (!string.IsNullOrWhiteSpace(FederatedApplicationSettings.AuthenticationErrorUrl))
            {
                filterContext.Result = new RedirectResult(GetUri(FederatedApplicationSettings.AuthenticationErrorUrl).AbsoluteUri);
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult(error);
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
                if (IsPostback)
                {

                    if (!IsValidPostback)
                    {
                        OnAuthenticationError(filterContext, $"Postback missing {AuthenticationRequestFormParameter} form value", $"External Authentication failed to return the required {AuthenticationRequestFormParameter} form field");
                        return;
                    }
                    try
                    {
                        string authenticationRequestToken =  TokenProvider.CreateToken(claims => {
                            claims.Add("ConsumerAuthenticationApiUrl", new string[] { ConsumerAuthenticationApiFormParam.Decrypt() });
                        });

                        HttpContext.SetAuthenticationRequestTokenCookieSuffix(CreateAuthenticationRequestTokenCookieSuffix());
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
                        if(loginRequestToken.Decrypt() is string authenticationRequestTokenCookieSuffix)
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

        private string GetAuthenticationRequestCookieValue() => GetAuthenticationRequestCookie() is HttpCookie cookie ? cookie.Value : null;
        private HttpCookie GetAuthenticationRequestCookie() => HasValidHttpCookie(AuthenticationRequestCookieName) ? GetHttpCookie(AuthenticationRequestCookieName) : null;
        private void RemoveAuthenticationRequestCookie() => RemoveHttpCookie(GetAuthenticationRequestCookie(), true);
        

    }
}
