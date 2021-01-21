using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Models;
using FederatedIPAuthenticationService.Services;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace FederatedIPAuthenticationService.Web.ConsumerAPI
{
    [ConsumerAuthenticationApiFilter]
    public abstract class ConsumerAuthenticationApiControllerBase : ApiController
    {
        protected IServices Services => HttpContext.Current.ApplicationInstance is IMvcServiceApplication app ? app.Services : null;
        protected ITokenProvider TokenProvider => Services.Get<ITokenProvider>();
        protected ISiteMeta SiteMeta => Services.Get<ISiteMeta>();

        protected HttpResponseMessage TextResponse(string value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(value);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
    }
    public abstract class ConsumerAuthenticationApiAuthenticationController : ConsumerAuthenticationApiControllerBase
    {
        [System.Web.Http.HttpPost]
        public ConsumerApiAuthenticationResponse Post([FromBody] string authReuestToken)
        {
            IDictionary<string, IEnumerable<string>> authReuestTokenClaims = TokenProvider.GetTokenClaims(authReuestToken);
            IEnumerable<string> providerAuthenticationCredentialsClaim = authReuestTokenClaims.ContainsKey("providerAuthenticationCredentials") ? authReuestTokenClaims["providerAuthenticationCredentials"] : null;
            IEnumerable<string> externalAuthAuthorizedUser = authReuestTokenClaims.ContainsKey("externalAuthAuthorizedUser") ? authReuestTokenClaims["externalAuthAuthorizedUser"] : null;


            if (providerAuthenticationCredentialsClaim == null && externalAuthAuthorizedUser == null)
            {
                return new ConsumerApiAuthenticationResponse() { Message = "No User Credentials" };
            }
            ProviderAuthenticationCredentials providerAuthenticationCredentials = new ProviderAuthenticationCredentials(providerAuthenticationCredentialsClaim);
            ConsumerUser authenticatedUser = ResolveAuthenticatedUser(providerAuthenticationCredentials, externalAuthAuthorizedUser);

            if (authenticatedUser != null)
            {
                return CreateConsumerAPISuccessResponse(authenticatedUser);
            }
            return new ConsumerApiAuthenticationResponse() { Message = "Invalid User" };
        }
        private ConsumerApiAuthenticationResponse CreateConsumerAPISuccessResponse(ConsumerUser authenticatedUser)
        {
            var token = TokenProvider.CreateToken(authTokenClaims => {
                authTokenClaims.AddUpdate("UserGuid", authenticatedUser.Guid.ToString());
                foreach (KeyValuePair<string, string> siteMetaItem in SiteMeta.Collection)
                {
                    authTokenClaims.AddUpdate(siteMetaItem.Key, siteMetaItem.Value.ToString());
                }
            });
            return new ConsumerApiAuthenticationResponse()
            {
                AuthenticationToken = token,
                Message = $"{authenticatedUser.Name}",
                AuthenticationTokenExpiration = TokenProvider.GetExpirationDate(token)
            };
        }
        protected abstract ConsumerUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials, IEnumerable<string> externalAuthAuthorizedUser);
    }
    public abstract class ConsumerAuthenticationApiTestUsersController : ConsumerAuthenticationApiControllerBase
    {
        [System.Web.Http.HttpGet]
        public IEnumerable<ConsumerUser> Get() => SiteMeta.AuthenticationModes.Contains("Test") ? ResolveTestUsers() : new ConsumerUser[0];
        protected abstract IEnumerable<ConsumerUser> ResolveTestUsers();
    }
    public abstract class ConsumerAuthenticationApiPrivacyNoticeController : ConsumerAuthenticationApiControllerBase
    {
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get() => TextResponse(GetPrivacyNotice());
        protected abstract string GetPrivacyNotice();
    }
    public abstract class ConsumerAuthenticationApiSiteMetaController : ConsumerAuthenticationApiControllerBase
    {
        [System.Web.Http.HttpGet]
        public ISiteMeta Get() => SiteMeta;
    }
    internal class ConsumerApiIdentity : IIdentity
    {
        public string Name { get; private set; }

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated { get; private set; }
        public static IIdentity Create(string name) => new ConsumerApiIdentity { Name = name, AuthenticationType = "Provider Access", IsAuthenticated = true };

    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ConsumerAuthenticationApiFilter : AuthorizationFilterAttribute
    {
        bool Active = true;

        public ConsumerAuthenticationApiFilter()
        { }


        public ConsumerAuthenticationApiFilter(bool active)
        {
            Active = active;
        }

        private string GetAuthorizationHeader(HttpActionContext actionContext, string scheme)
       => actionContext.Request.Headers.Authorization is AuthenticationHeaderValue auth && auth != null && auth.Scheme == scheme ? auth.Parameter : null;

        private string GetBearerAuthorizationHeader(HttpActionContext actionContext)
        => GetAuthorizationHeader(actionContext, "Bearer");

        private IServices Services => HttpContext.Current.ApplicationInstance is IMvcServiceApplication app ? app.Services : null;
        private ITokenProvider TokenProvider => Services.Get<ITokenProvider>();


        public override void OnAuthorization(HttpActionContext actionContext)
        {

            if (Active)
            {
                IIdentity identity = null;
                string scheme = "Bearer";
                if (GetBearerAuthorizationHeader(actionContext) is string token && !string.IsNullOrWhiteSpace(token))
                {
                    identity = OnTokenAuthorizeUser(token);
                }

                if (identity == null)
                {
                    Challenge(actionContext, scheme);
                    return;
                }

                var principal = new GenericPrincipal(identity, null);

                // inside of ASP.NET this is required
                if (HttpContext.Current != null)
                    HttpContext.Current.User = principal;

                base.OnAuthorization(actionContext);
            }
        }


        private IIdentity OnTokenAuthorizeUser(string token)
        {
            if (TokenProvider.IsValidToken(token) && TokenProvider.GetTokenClaims(token).ContainsKey("Provider"))
            {
                return ConsumerApiIdentity.Create("Authentication Provider");
            }
            return null;
        }

        private void Challenge(HttpActionContext actionContext, string scheme)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", $"{scheme} realm=\"{host}\"");
        }
    }
    public static class ConsumerAuthenticationApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "AuthenticationApi",
                routeTemplate: "authenticationApi/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
