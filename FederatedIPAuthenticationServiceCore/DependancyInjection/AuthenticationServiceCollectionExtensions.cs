using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace FederatedIPAuthenticationServiceCore.DependancyInjection
{
    //
    // Summary:
    //     Extension methods for setting up authentication services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    public static class AuthenticationServiceCollectionExtensions
    {
        internal static IDictionary<string, string> GetSettings(this IConfiguration configuration, string section)
        =>  configuration.GetSection(section).GetChildren().ToDictionary(x=> x.Key, x=> x.Value.ToString());
        internal static TService AddSingletonAndReturnInstance<TService>(this IServiceCollection services, TService instance)
            where TService : class
        {
            services.AddSingleton(instance);
            return instance;
        }

        public static AuthenticationBuilder AddFederatedConsumerAuthentication(this IServiceCollection services, IConfiguration configuration, Action<AuthenticationOptions> configureOptions)
        {
            //services.AddSingleton(authenticationEvents);
            var federatedConsumerSettings = services.AddSingletonAndReturnInstance(FederatedSettings.GetConsumerSettings(section => configuration.GetSettings(section)));
            var siteMeta = services.AddSingletonAndReturnInstance(FederatedSettings.GetSiteMeta(section => configuration.GetSettings(section)));
            var tokenProviderSettings = services.AddSingletonAndReturnInstance(FederatedSettings.GetTokenProviderSettings(section => configuration.GetSettings(section)));

            var tokenProvider = services.AddSingletonAndReturnInstance(ServiceFactory.CreateTokenProvider(tokenProviderSettings));

            FederatedIPAuthenticationConsumerEvents authenticationEvents = new FederatedIPAuthenticationConsumerEvents(federatedConsumerSettings, siteMeta, tokenProvider);

            AuthenticationBuilder builder = services.AddAuthentication(authenticationEvents.AuthenticationCookieName)
            .AddCookie(authenticationEvents.AuthenticationCookieName, config =>
             {
                 cookieOptions(config);
                 config.Events = authenticationEvents;
                 if (string.IsNullOrWhiteSpace(config.Cookie.Name))
                 {
                     config.Cookie.Name = FederatedIPDefaults.CookieName;
                 }
                 if (config.LoginPath == null)
                 {
                     throw new ArgumentNullException(nameof(config.LoginPath));
                 }
                 if (config.LogoutPath == null)
                 {
                     throw new ArgumentNullException(nameof(config.LogoutPath));
                 }
             });
        }
        //public static AuthenticationBuilder AddAuthentication(this IServiceCollection services, string defaultScheme);


    }
    public class FederatedIPAuthenticationConsumerEvents : CookieAuthenticationEvents
    {
        protected CookieValidatePrincipalContext CookieValidatePrincipalContext { get; set; }
        protected UrlHelper Url { get; set; }
        protected HttpContext HttpContext { get => CookieValidatePrincipalContext.HttpContext; }
        
        protected HttpRequest Request { get => CookieValidatePrincipalContext.Request; }
        protected RouteValueDictionary RouteData { get => Request.RouteValues; }
        private bool IsSecureRequest { get => Request.Scheme.ToLower().Trim() == "https"; }
        protected HttpResponse Response { get => CookieValidatePrincipalContext.Response; }
        protected ClaimsPrincipal User { get => HttpContext.User; }
        protected IIdentity Identity { get => User == null ? null : User.Identity; }
        protected bool IsAuthenticated { get => Identity != null && Identity.IsAuthenticated; }
        protected bool IsPostRequest { get => Request.Method == "POST"; }
        public bool IsAuthenticatedRoute { get; private set; }

        private IFederatedConsumerSettings FederatedConsumerSettings { get; set; }
        private ISiteMeta SiteMeta { get; set; }
        private bool UseRhealm { get => !string.IsNullOrWhiteSpace(SiteMeta.SiteRhealm); }
        private ITokenProvider TokenProvider { get; set; }
        private Uri LogoutUri => GetUri(FederatedConsumerSettings.LogoutUrl);
        private bool IsLogoutRequest { get => Request.Path.GetDisplayUrl().GetLeftPart(UriPartial.Path) == LogoutUri.GetLeftPart(UriPartial.Path); }
        private bool IsTokenPostRequest { get => IsPostRequest && Request.Form[Extensions.HtmlExtensions.FederatedIPAuthTokenParam] is string postToken && !string.IsNullOrWhiteSpace(postToken); }
        internal string AuthenticationCookieName { get => $"{FederatedSettings.FederatedAuthenticationTokenCookiePrefix}{(UseRhealm ? SiteMeta.SiteRhealm : SiteMeta.SiteId)}"; }
        private string AuthenticationRequestCookieName { get => $"{FederatedSettings.FederatedAuthenticationRequestTokenCookiePrefix}{FederatedConsumerSettings.AuthenticationProviderId}"; }
        public FederatedIPAuthenticationConsumerEvents(IFederatedConsumerSettings federatedConsumerSettings, ISiteMeta siteMeta, ITokenProvider tokenProvider)
        {
            FederatedConsumerSettings = federatedConsumerSettings;
            SiteMeta = siteMeta;
            TokenProvider = tokenProvider;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            ClaimsPrincipal currentUser = context.Principal;

            string token = context.Principal.GetClaimValue(ClaimTypes.Authentication);

            TokenResponse tokenResponse = await WebAPIClient.ValidateTokenAsync(token);
            if (!tokenResponse.IsValid)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(_authenticationScheme);
            }
            else
            {
                IEnumerable<Claim> userClaimsForToken = currentUser.Claims.Where(claim => claim.Type != ClaimTypes.Authentication);
                tokenResponse = await WebAPIClient.RenewTokenAsync(token, userClaimsForToken.Select(c => new UserIdentityClaim(c)));

                context.Principal.AddUpdateClaim(ClaimTypes.Authentication, tokenResponse.Token);
                await context.HttpContext.SignInAsync(context.Principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = tokenResponse.ValidTo
                });
            }
        }

        /*custom methods*/

        private async Task<ClaimsPrincipal> CreateUserPrincipal(Guid UserGuid)
        {
            var userIdentity = new ClaimsIdentity(new List<Claim>(), "SAML2 Token User Identity", ClaimTypes.Name, ClaimTypes.Role);
            var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });

            /*GET/SET USER INFO HERE*/
            userPrincipal.AddUpdateClaim(ClaimTypes.Name, "John Doe");
            userPrincipal.AddUpdateClaim(ClaimTypes.NameIdentifier, UserGuid.ToString());
            userPrincipal.AddUpdateClaim(ClaimTypes.Email, "john.doe@test.com");
            userPrincipal.AddClaim(ClaimTypes.Role, "flex");
            userPrincipal.AddClaim(ClaimTypes.Role, "admin");
            userPrincipal.AddClaim(ClaimTypes.Role, "test");

            return await Task.Run(() => userPrincipal);
        }
        public async Task SignIn(Guid UserGuid, HttpContext context)
        {
            ClaimsPrincipal userPrincipal = await CreateUserPrincipal(UserGuid);
            IEnumerable<Claim> userClaimsForToken = userPrincipal.Claims.Where(claim => claim.Type != ClaimTypes.Authentication);
            TokenResponse tokenResponse = await WebAPIClient.RequestTokenAsync(userClaimsForToken.Select(c => new UserIdentityClaim(c)));
            if (tokenResponse.IsValid)
            {

                userPrincipal.AddUpdateClaim(ClaimTypes.Authentication, tokenResponse.Token);
                AuthenticationProperties authProp = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = tokenResponse.ValidTo
                };
                await context.SignInAsync(userPrincipal, authProp);
            }
            else
            {
                await SignOut(context);
            }

        }
        public async Task SignOut(HttpContext context)
        {
            await context.SignOutAsync(_authenticationScheme);
        }
    }

}
