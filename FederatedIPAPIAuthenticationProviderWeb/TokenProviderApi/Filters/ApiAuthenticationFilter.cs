using FederatedIPAPIAuthenticationProviderWeb.Services;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Web;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FederatedIPAPIAuthenticationProviderWeb.TokenProviderApi.Filters
{
    public class AcceptHeaderJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
    internal class ApiIdentity : IIdentity
    {
        public string Name { get; private set; }

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated { get; private set; }
        public static IIdentity Create(string name, string authenticationType) => new ApiIdentity { Name = name, AuthenticationType = authenticationType, IsAuthenticated = true };

    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiAuthenticationFilter : AuthorizationFilterAttribute
    {
        bool Active = true;
        bool BasicAuth = false;

        public ApiAuthenticationFilter()
        { }

        
        public ApiAuthenticationFilter(bool basicAuth, bool active=true)
        {
            BasicAuth = basicAuth;
            Active = active;
        }

        private string GetAuthorizationHeader(HttpActionContext actionContext, string scheme)
       => actionContext.Request.Headers.Authorization is AuthenticationHeaderValue auth && auth != null && auth.Scheme == scheme ? auth.Parameter : null;
        private string GetBasicAuthorizationHeader(HttpActionContext actionContext)
        => GetAuthorizationHeader(actionContext, "Basic");
        private string GetBearerAuthorizationHeader(HttpActionContext actionContext)
        => GetAuthorizationHeader(actionContext, "Bearer");

        private IServices Services => HttpContext.Current.ApplicationInstance is IMvcServiceApplication app ? app.Services : null;
        private ITokenProviderService TokenProviderService => Services.Get<ITokenProviderService>();
        

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            
            if (Active)
            {
                IIdentity identity = null;
                string scheme = "Basic";
                if (BasicAuth && GetBasicAuthorizationHeader(actionContext) is string basicAuthHeader && !string.IsNullOrWhiteSpace(basicAuthHeader))
                {
                    var credentials = GetBasicCredentials(basicAuthHeader);
                    identity = OnBasicAuthorizeUser(credentials.username, credentials.password);
                }
                else if (GetBearerAuthorizationHeader(actionContext) is string token && !string.IsNullOrWhiteSpace(token))
                {
                    identity = OnTokenAuthorizeUser(token);
                    scheme = "Bearer";                   
                }
                
                if (identity == null)
                {
                    Challenge(actionContext, scheme);
                    return;
                }

                var principal = new GenericPrincipal(identity, null);

                //Thread.CurrentPrincipal = principal;

                // inside of ASP.NET this is required
                if (HttpContext.Current != null)
                    HttpContext.Current.User = principal;

                base.OnAuthorization(actionContext);
            }
        }

        
        private IIdentity OnBasicAuthorizeUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            return ApiIdentity.Create("Test Basic","Basic");
        }
        private IIdentity OnTokenAuthorizeUser(string token)
        {
            if (TokenProviderService.IsValid(token) && TokenProviderService.GetClaims(token).FirstOrDefault(clm => clm.Name == "Name") is TokenClaim claim) {
                return ApiIdentity.Create(claim.Value, "Token");
            }
            return null;
        }
        protected virtual (string username, string password) GetBasicCredentials(string authHeader)
        {
            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2)
                return (null, null);

            return (tokens[0], tokens[1]);
        }

        void Challenge(HttpActionContext actionContext, string scheme)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", $"{scheme} realm=\"{host}\"");
        }

    }
}