using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Principal;
using FederatedIPAuthenticationService.ServiceProvider;
using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.Web.Application;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FederatedIPAuthenticationService.Attributes
{
    public abstract class FederatedAuthenticationConsumer : FederatedAuthenticationFilter
    {
        protected IServices Services { get => HttpContext.ApplicationInstance is IMvcServiceApplication app? app.Services : null; }
        private IFederatedConsumerSettings FederatedConsumerSettings => Services.Get<IFederatedConsumerSettings>();
        private ISiteMeta SiteMeta => Services.Get<ISiteMeta>();
        private bool UseRhealm { get => !string.IsNullOrWhiteSpace(SiteMeta.SiteRhealmId); }
        private ITokenProvider TokenProvider => Services.Get<ITokenProvider>();
        private Uri LogoutUri => GetUri(FederatedConsumerSettings.LogoutUrl);
        private bool IsLogoutRequest { get => Request.Url.GetLeftPart(UriPartial.Path) == LogoutUri.GetLeftPart(UriPartial.Path); }
        private string AuthenticationCookieName { get => $"{FederatedSettings.FederatedAuthenticationTokenCookiePrefix}{(UseRhealm ? SiteMeta.SiteRhealmId : SiteMeta.SiteId)}"; }
        private string AuthenticationRequestCookieName { get => $"{FederatedSettings.FederatedAuthenticationRequestTokenCookiePrefix}{FederatedConsumerSettings.AuthenticationProviderId}"; }
        public FederatedAuthenticationConsumer(bool isAuthenticatedRoute = true) : base(isAuthenticatedRoute) { }

        protected abstract IIdentity CreateAuthenticatedPrincipalIdentity(IDictionary<string, IEnumerable<string>> tokenClaims);
        protected abstract IEnumerable<string> GetRoles(IDictionary<string, IEnumerable<string>> tokenClaims, IEnumerable<string> currentRoles);
        protected virtual void OnLogout(IIdentity identity) { }
        protected virtual void SetTokenUpdateClaims(IIdentity identity, IDictionary<string, IEnumerable<string>> tokenClaims) { }

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
                OnAuthenticatedRequest(filterContext);
            }
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
            HttpContext.Session.Add("LogoutRedirectUrl", FederatedConsumerSettings.AuthenticationProviderUrl);
            RemoveAuthenticationCookie();
            HttpContext.User = FederatedPrincipal.CreateLogout();
            string authenticationRequestToken = TokenProvider.CreateToken(claims => {
                foreach (KeyValuePair<string, string> siteMetaItem in SiteMeta.Collection)
                {
                    claims.AddUpdate(siteMetaItem.Key, siteMetaItem.Value.ToString());
                }
            });
            SetAuthenticationRequestTokenCookie(AuthenticationRequestCookieName, authenticationRequestToken, (DateTime)TokenProvider.GetExpirationDate(authenticationRequestToken));
        }

        /* Process Authenticated Request
         * 1. Get Auth Token value
         * 2. Check if token was provided via a POST back option
         * 3. If token is a valid token 
         *      Create Identity, set Authenticated Principal, and update site auth cookie
         * 4. Else Logout: 
         *      a.If Request is an AJAX request then complete the request with a HttpUnauthorizedResult. 
         *          client AJAX handler is responsible for triggering a logout request on the client page;
         *      b. Else redirect the consuming app logout url. 
         *          (This will begin a new authentication check process with the IsLogoutRequest set to true)
         */
        private void OnAuthenticatedRequest(AuthenticationContext filterContext)
        {
            string token = GetAuthenticationCookieValue();
            if (!string.IsNullOrWhiteSpace(token) && 
                TokenProvider.IsValidToken(token) && 
                TokenProvider.GetTokenClaims(token) is IDictionary<string,IEnumerable<string>> tokenClaims &&
                FederatedSettings.GetSiteMeta(tokenClaims) is SiteMeta siteMetaFromToken &&
                (siteMetaFromToken.SiteRhealmId == SiteMeta.SiteRhealmId || siteMetaFromToken.SiteId == SiteMeta.SiteId))
            {
                try {
                    IIdentity identity = CreateAuthenticatedPrincipalIdentity(tokenClaims);
                    string updatedToken = TokenProvider.RenewToken(token, (claims) =>
                    {
                        claims.AddUpdate("Roles", GetRoles(tokenClaims, tokenClaims.Get<string>("Roles")));
                        foreach (KeyValuePair<string, string> siteMetaItem in SiteMeta.Collection)
                        {
                            claims.AddUpdate(siteMetaItem.Key, siteMetaItem.Value.ToString());
                        }
                        SetTokenUpdateClaims(identity, claims);
                    });
                    var updatedTokenClaims = TokenProvider.GetTokenClaims(updatedToken);
                    DateTime? expirationDate = TokenProvider.GetExpirationDate(updatedToken);
                    HttpContext.User = FederatedPrincipal.Create(updatedTokenClaims.Get<string>("Roles"), expirationDate, identity);
                    SetAuthenticationCookie(updatedToken, expirationDate);
                }
                catch
                {
                    UriBuilder builder = new UriBuilder(LogoutUri);
                    filterContext.Result = new RedirectResult(builder.Uri.ToString());
                }
                
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    filterContext.Result = new HttpUnauthorizedResult("Not Authenticated");
                }
                else
                {
                    OnLogout(HttpContext.User.Identity);
                    UriBuilder builder = new UriBuilder(LogoutUri);
                    filterContext.Result = new RedirectResult(builder.Uri.ToString());
                }
            }
        }


        private string GetAuthenticationCookieValue() => GetAuthenticationCookie() is HttpCookie cookie ? cookie.Value : null;
        private HttpCookie GetAuthenticationCookie() => HasValidHttpCookie(AuthenticationCookieName) ? GetHttpCookie(AuthenticationCookieName) : null;
        private void RemoveAuthenticationCookie() => RemoveHttpCookie(GetAuthenticationCookie());

        /*Authentication still checks for valid experation date in token*/
        private void SetAuthenticationCookie(string value, DateTime? expires = null)
        {
            if (FederatedConsumerSettings.UseSessionCookie)
            {
                SetHttpCookie(CreateSessionCookie(AuthenticationCookieName, value, !UseRhealm));
            }
            else if (expires is DateTime expirationDate)
            {
                SetHttpCookie(CreateCookie(AuthenticationCookieName, value, expirationDate, !UseRhealm));
            }
        }
    }
}
