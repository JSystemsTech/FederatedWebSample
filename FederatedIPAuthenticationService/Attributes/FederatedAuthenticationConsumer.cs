using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Extensions;
using FederatedAuthNAuthZ.Principal;
using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Services;
using ServiceProviderShared;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FederatedAuthNAuthZ.Attributes
{
    public class FederatedApplicationAttribute : FederatedAuthenticationFilter
    {
        private IFederatedApplicationSettings FederatedApplicationSettings => ServiceManager.GetService<IFederatedApplicationSettings>();
        private IFederatedApplicationIdentityService FederatedApplicationIdentityService => ServiceManager.GetService<IFederatedApplicationIdentityService>();
        private ITokenProvider TokenProvider => ServiceManager.GetService<ITokenProvider>();
        private IEncryptionService EncryptionService => ServiceManager.GetService<IEncryptionService>();
        private Uri LogoutUri => GetUri(FederatedApplicationSettings.LogoutUrl);
        private bool IsLogoutRequest { get => Request.Url.GetLeftPart(UriPartial.Path) == LogoutUri.GetLeftPart(UriPartial.Path); }
        private string AuthenticationCookieName { get => $"{FederatedApplicationSettings.GetCookiePrefix()}{FederatedApplicationSettings.GetCookieSuffix()}"; }
        public FederatedApplicationAttribute(bool isAuthenticatedRoute = true) : base(isAuthenticatedRoute) { }


        /* Process Request 
         * 1.If Request is a logout request handle that request
         * 2. If Request is tagged as an Authenticated Route then check for valid Authentication credendials
         * 3. Else Assume Request is to a url that does not require authentication to process.
         */
        public sealed override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
            if (IsLogoutRequest)
            {
                OnLogoutRequest(filterContext);
            }
            else if (IsAuthenticatedRoute)
            {
                HandleGeneralRequest(filterContext, HandleLogoutRequest, HandleBadTokenRequest);
            }
            HandleGeneralRequest(filterContext, fc => { },fc => { });
        }
        /* Process Logout Request
         * 1. If Logout Request does not contail a query parameter then redirect back to logout url with default query
         *      (this is by design to allow for simple logout links on the page)
         * 2. Else Complete Logout 
         *  a. Remove Auth token
         *  b. Set the Principal to Logout principal
         *  c. Build Authentication Request token
         *  d. set Authentication Request Token Cookie to Authentication Request token value
         *  e. Complete the Request (requst action should build a page that redirects the user to the provided 'LogoutRedirectUrl' session variable)
         */
        private void OnLogoutRequest(AuthenticationContext filterContext) 
        {
            string AuthenticationRequestTokenCookieSuffix = CreateAuthenticationRequestTokenCookieSuffix();
            HttpContext.Session.Add("LogoutRedirectUrl", $"{FederatedApplicationSettings.AuthenticationProviderUrl}/{FederatedApplicationSettings.SiteId}?LoginRequestToken={Url.Encode(EncryptionService.DateSaltEncrypt(AuthenticationRequestTokenCookieSuffix))}");
            
            
            RemoveAuthenticationCookie();
            HttpContext.User = FederatedPrincipal.CreateLogout();
            string authenticationRequestToken = TokenProvider.CreateToken(claims => {
                claims.AddUpdate("ConsumerAuthenticationApiUrl", FederatedApplicationSettings.ConsumerAuthenticationApiUrl);
            });
            SetAuthenticationRequestTokenCookie(FederatedApplicationSettings.GetAuthRequestCookieName() + AuthenticationRequestTokenCookieSuffix, authenticationRequestToken, (DateTime)TokenProvider.GetExpirationDate(authenticationRequestToken));
        }

        private void HandleLogoutRequest(AuthenticationContext filterContext)
        {
            UriBuilder builder = new UriBuilder(LogoutUri);
            filterContext.Result = new RedirectResult(builder.Uri.ToString());
        }
        private void HandleBadTokenRequest(AuthenticationContext filterContext)
        {
            if (Request.IsAjaxRequest())
            {
                filterContext.Result = new HttpUnauthorizedResult("Not Authenticated");
            }
            else
            {
                FederatedApplicationIdentityService.OnLogout(HttpContext.User.Identity);
                HandleLogoutRequest(filterContext);
            }
        }
        private void HandleGeneralRequest(AuthenticationContext filterContext, Action<AuthenticationContext> handleLogoutRequest, Action<AuthenticationContext> handleBadTokenRequest)
        {
            string token = GetAuthenticationCookieValue();
            if (!string.IsNullOrWhiteSpace(token) &&
                TokenProvider.IsValidToken(token) &&
                TokenProvider.GetTokenClaims(token) is IDictionary<string, IEnumerable<string>> tokenClaims &&
                FederatedApplicationSettings.ValidateConsumerTokenClaims(tokenClaims)
                )
            {
                try
                {
                    IIdentity identity = FederatedApplicationIdentityService.CreateAuthenticatedPrincipalIdentity(tokenClaims);
                    string updatedToken = TokenProvider.RenewToken(token, (claims) =>
                    {
                        claims.AddUpdate("Roles", FederatedApplicationIdentityService.GetRoles(tokenClaims, tokenClaims.Get<string>("Roles")));
                        FederatedApplicationSettings.UpdateConsumerTokenClaims(tokenClaims);
                        FederatedApplicationIdentityService.SetTokenUpdateClaims(identity, claims);
                    });
                    var updatedTokenClaims = TokenProvider.GetTokenClaims(updatedToken);
                    DateTime? expirationDate = TokenProvider.GetExpirationDate(updatedToken);
                    HttpContext.User = FederatedPrincipal.Create(updatedTokenClaims.Get<string>("Roles"), expirationDate, identity);
                    SetAuthenticationCookie(updatedToken, expirationDate);
                }
                catch
                {
                    handleLogoutRequest(filterContext);
                }

            }
            else
            {
                handleBadTokenRequest(filterContext);
            }
        }
        private string GetAuthenticationCookieValue() => GetAuthenticationCookie() is HttpCookie cookie ? cookie.Value : null;
        private HttpCookie GetAuthenticationCookie() => HasValidHttpCookie(AuthenticationCookieName) ? GetHttpCookie(AuthenticationCookieName) : null;
        private void RemoveAuthenticationCookie() => RemoveHttpCookie(GetAuthenticationCookie());

        /*Authentication still checks for valid experation date in token*/
        private void SetAuthenticationCookie(string value, DateTime? expires = null)
        {
            if (FederatedApplicationSettings.UseSessionCookie)
            {
                SetHttpCookie(CreateSessionCookie(AuthenticationCookieName, value, !FederatedApplicationSettings.UseRealm()));
            }
            else if (expires is DateTime expirationDate)
            {
                SetHttpCookie(CreateCookie(AuthenticationCookieName, value, expirationDate, !FederatedApplicationSettings.UseRealm()));
            }
        }
    }
}
