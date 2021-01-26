using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Models;
using FederatedIPAuthenticationService.Services;
using Newtonsoft.Json;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Web;
using System;
using System.Collections.Generic;
using System.IO;
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
        protected IFederatedApplicationSettings FederatedApplicationSettings => Services.Get<IFederatedApplicationSettings>();
        private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        protected HttpResponseMessage TextResponse(string value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(value);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
        protected HttpResponseMessage EncryptedResponse<T>(T model)
        {
            string value = EncryptionService.DateSaltEncrypt(model is string strModel ? strModel : JsonConvert.SerializeObject(model));
            return TextResponse(value);
        }
        protected string DecryptBody(string bodyValue) => EncryptionService.DateSaltDecrypt(bodyValue, true);
    }
    public class EncryptedPostBody
    {
        public string Content { get; set; }
    }
    public abstract class ConsumerAuthenticationApiAuthenticationController : ConsumerAuthenticationApiControllerBase
    {
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post([FromBody] EncryptedPostBody body)
        {
            string decryptedBody = DecryptBody(body.Content);

            ConsumerApiAuthenticationResponse responseObj = new ConsumerApiAuthenticationResponse() { Message = "No User Credentials" };
            if (decryptedBody.TryDeserializeObject(out ProviderAuthenticationCredentials authRequest) && 
                (authRequest.Username != null || 
                authRequest.Password != null || 
                authRequest.Email != null ||
                authRequest.TestUserGuid != null || 
                authRequest.UserData != null))
            {
                responseObj = ResolveAuthenticatedUser(authRequest) is ConsumerUser authenticatedUser ? CreateConsumerAPISuccessResponse(authenticatedUser) : 
                    new ConsumerApiAuthenticationResponse() { Message = "Invalid User" };                
            }

            return EncryptedResponse(responseObj);
        }
        private ConsumerApiAuthenticationResponse CreateConsumerAPISuccessResponse(ConsumerUser authenticatedUser)
        {
            var token = TokenProvider.CreateToken(authTokenClaims => {
                authTokenClaims.AddUpdate("UserGuid", authenticatedUser.Guid.ToString());
                FederatedApplicationSettings.UpdateConsumerTokenClaims(authTokenClaims);
            });
            return new ConsumerApiAuthenticationResponse()
            {
                AuthenticationToken = token,
                Message = $"{authenticatedUser.Name}",
                AuthenticationTokenExpiration = TokenProvider.GetExpirationDate(token)
            };
        }
        protected abstract ConsumerUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials);
    }

    public interface IConsumerApplicationSettingsResponse
    {
        IEnumerable<ConsumerUser> TestUsers { get; set; }
        FederatedApplicationSettings FederatedApplicationSettings { get; }
        string LogoImage { get; }
    }
    public class ConsumerApplicationSettingsResponse: IConsumerApplicationSettingsResponse
    {
        public IEnumerable<ConsumerUser> TestUsers { get; set; }
        public FederatedApplicationSettings FederatedApplicationSettings { get; set; }
        public string LogoImage { get; set; }
    }
    public abstract class ConsumerAuthenticationApiConsumerApplicationSettingsController : ConsumerAuthenticationApiControllerBase
    {
        private static IEnumerable<ConsumerUser> EmptyTestUsers = new ConsumerUser[0];
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get() => EncryptedResponse(new ConsumerApplicationSettingsResponse() { 
            TestUsers = FederatedApplicationSettings.AuthenticationModes.Contains("Test") ? GetTestUsers() : EmptyTestUsers,
            FederatedApplicationSettings = new FederatedApplicationSettings(FederatedApplicationSettings),
            LogoImage = GetLogoImage()
        });
        protected virtual IEnumerable<ConsumerUser> GetTestUsers() => EmptyTestUsers;
        protected virtual string GetLogoImage() => null;
        protected string LoadImageFromFile(string path)
        {
            try
            {                
                string fullPath = HttpContext.Current.Server.MapPath(path);
                System.Drawing.Image image = System.Drawing.Image.FromFile(fullPath);
                using (var ms = new System.IO.MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    var base64 = Convert.ToBase64String(ms.ToArray());
                    return $"data:image/gif;base64,{base64}";
                }
            }
            catch(Exception e)
            {
                return null;
            }
            
        }
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
