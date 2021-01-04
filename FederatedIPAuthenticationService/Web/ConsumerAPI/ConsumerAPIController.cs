using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebApiClientShared.Web;

namespace FederatedIPAuthenticationService.Web.ConsumerAPI
{
    public abstract class ConsumerAPIController: FederatedWebController
    {
        protected override string GetDeafaultNamespace() => null;
        protected override void OnActionExecuting(ActionExecutingContext filterContext) { }

        private static string AuthorizationHeaderPrefix = "Bearer ";
        private string AuthorizationHeader { get => Request.Headers.GetValues("Authorization").FirstOrDefault(); }
        private string AuthorizationHeaderToken { get => AuthorizationHeader is string value && value.StartsWith(AuthorizationHeaderPrefix) ? value.Remove(0, AuthorizationHeaderPrefix.Length) : ""; }
        private IDictionary<string, IEnumerable<string>> HeaderTokenClaims { get; set; }
        protected sealed override void OnAuthentication(AuthenticationContext filterContext) {

            bool isValidToken = !string.IsNullOrWhiteSpace(AuthorizationHeaderToken) &&
                TokenProvider.IsValidToken(AuthorizationHeaderToken) &&
                TokenProvider.GetTokenClaims(AuthorizationHeaderToken) is IDictionary<string, IEnumerable<string>> tokenClaims &&
                FederatedSettings.GetSiteMeta(tokenClaims) is SiteMeta siteMetaFromToken &&
                (siteMetaFromToken.SiteRhealmId == SiteMeta.SiteRhealmId || siteMetaFromToken.SiteId == SiteMeta.SiteId);
            if (!isValidToken)
            {
                filterContext.Result = Json(new { }, JsonRequestBehavior.DenyGet);
                return;
            }
            HeaderTokenClaims = TokenProvider.GetTokenClaims(AuthorizationHeaderToken);
        }
        public JsonResult GetTestUsers()
        {
            return Json(ResolveTestUsers(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Authenticate()
        {
            IEnumerable<string> providerAuthenticationCredentialsClaim = HeaderTokenClaims.ContainsKey("providerAuthenticationCredentials") ? HeaderTokenClaims["providerAuthenticationCredentials"] : null;
            IEnumerable<string> externalAuthAuthorizedUser = HeaderTokenClaims.ContainsKey("externalAuthAuthorizedUser") ? HeaderTokenClaims["externalAuthAuthorizedUser"] : null;

            if (providerAuthenticationCredentialsClaim == null && externalAuthAuthorizedUser == null)
            {
                return Json(new ConsumerApiAuthenticateResponse() { Message = "No User Credentials" }, JsonRequestBehavior.AllowGet);
            }
            ProviderAuthenticationCredentials providerAuthenticationCredentials = new ProviderAuthenticationCredentials(providerAuthenticationCredentialsClaim);
            ConsumerUser authenticatedUser = ResolveAuthenticatedUser(providerAuthenticationCredentials, externalAuthAuthorizedUser);

            if (authenticatedUser != null)
            {
                return Json(CreateConsumerAPISuccessResponse(authenticatedUser), JsonRequestBehavior.AllowGet);
            }
            return Json(new ConsumerApiAuthenticateResponse() { Message = "Invalid User" }, JsonRequestBehavior.AllowGet);
        }
        protected abstract IEnumerable<ConsumerUser> ResolveTestUsers();
        protected abstract ConsumerUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials, IEnumerable<string> externalAuthAuthorizedUser);
    }
}
