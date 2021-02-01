using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Extensions;
using FederatedAuthNAuthZ.Models;
using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web;
using Newtonsoft.Json;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Web;
using ServiceProviderShared;
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

namespace FederatedAuthNAuthZ.Web.ConsumerAPI
{
    public interface IApplicationAuthenticationAPIApplicationSettingsResponse
    {
        IEnumerable<ApplicationUser> TestUsers { get; set; }
        FederatedApplicationSettings FederatedApplicationSettings { get; }
        string LogoImage { get; }
    }
    public class ApplicationAuthenticationAPIApplicationSettingsResponse : IApplicationAuthenticationAPIApplicationSettingsResponse
    {
        public IEnumerable<ApplicationUser> TestUsers { get; set; }
        public FederatedApplicationSettings FederatedApplicationSettings { get; set; }
        public string LogoImage { get; set; }
    }
    [ApplicationAuthenticationApi]
    public abstract class ApplicationAuthenticationAPIControllerBase : ApiControllerBase
    {
        protected ITokenProvider TokenProvider => ServiceManager.GetService<ITokenProvider>();
        protected IFederatedApplicationSettings FederatedApplicationSettings => ServiceManager.GetService<IFederatedApplicationSettings>();

        private ApplicationAuthenticationApiAuthenticationResponse CreateConsumerAPISuccessResponse(ApplicationUser authenticatedUser)
        {
            var token = TokenProvider.CreateToken(authTokenClaims => {
                authTokenClaims.AddUpdate("UserId", authenticatedUser.UserId);
                FederatedApplicationSettings.UpdateConsumerTokenClaims(authTokenClaims);
            });
            return new ApplicationAuthenticationApiAuthenticationResponse()
            {
                AuthenticationToken = token,
                Message = $"{authenticatedUser.Name}",
                AuthenticationTokenExpiration = TokenProvider.GetExpirationDate(token)
            };
        }
        protected abstract ApplicationUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials);
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Authentication([FromBody] EncryptedPostBody body)
        {
            string decryptedBody = DecryptBody(body.Content);

            ApplicationAuthenticationApiAuthenticationResponse responseObj = new ApplicationAuthenticationApiAuthenticationResponse() { Message = "No User Credentials" };
            if (decryptedBody.TryDeserializeObject(out ProviderAuthenticationCredentials authRequest) &&
                (authRequest.Username != null ||
                authRequest.Password != null ||
                authRequest.Email != null ||
                authRequest.TestUserId != null ||
                authRequest.UserData != null))
            {
                responseObj = ResolveAuthenticatedUser(authRequest) is ApplicationUser authenticatedUser ? CreateConsumerAPISuccessResponse(authenticatedUser) :
                    new ApplicationAuthenticationApiAuthenticationResponse() { Message = "Invalid User" };
            }

            return EncryptedResponse(responseObj);
        }


        private static IEnumerable<ApplicationUser> EmptyTestUsers = new ApplicationUser[0];
        [System.Web.Http.HttpGet]
        public HttpResponseMessage ApplicationSettings() => EncryptedResponse(new ApplicationAuthenticationAPIApplicationSettingsResponse()
        {
            TestUsers = FederatedApplicationSettings.AuthenticationModes.Contains("Test") ? GetTestUsers() : EmptyTestUsers,
            FederatedApplicationSettings = new FederatedApplicationSettings(FederatedApplicationSettings),
            LogoImage = GetLogoImage()
        });
        protected virtual IEnumerable<ApplicationUser> GetTestUsers() => EmptyTestUsers;
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
            catch (Exception e)
            {
                return null;
            }

        }
    }

    internal class ApplicationAuthenticationApiIdentity : IIdentity
    {
        public string Name { get; private set; }

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated { get; private set; }
        public static IIdentity Create(string name) => new ApplicationAuthenticationApiIdentity { Name = name, AuthenticationType = "Provider Access", IsAuthenticated = true };

    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApplicationAuthenticationApiAttribute : AuthorizationFilterAttribute
    {
        bool Active = true;

        public ApplicationAuthenticationApiAttribute()
        { }


        public ApplicationAuthenticationApiAttribute(bool active)
        {
            Active = active;
        }

        private string GetAuthorizationHeader(HttpActionContext actionContext, string scheme)
       => actionContext.Request.Headers.Authorization is AuthenticationHeaderValue auth && auth != null && auth.Scheme == scheme ? auth.Parameter : null;

        private string GetBearerAuthorizationHeader(HttpActionContext actionContext)
        => GetAuthorizationHeader(actionContext, "Bearer");

        private ITokenProvider TokenProvider => ServiceManager.GetService<ITokenProvider>();


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
                return ApplicationAuthenticationApiIdentity.Create("Authentication Provider");
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
}
