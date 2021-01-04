using System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using FederatedIPAPI.Configuration;
using FederatedIPAPI.Services;
using FederatedIPAPI.Extensions;

namespace FederatedIPAPI.BasicAuth
{
    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string _realm;
        private IAPISettings APISettings { get; set; }
        private IUserService UserService { get; set; }
        public BasicAuthFilter(string realm, IOptions<APISettings> apiSettings, IUserService userService)
        {
            _realm = realm;
            APISettings = apiSettings.Value;
            UserService = userService;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Please provide a non-empty realm value.");
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                if (authHeader != null)
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                    if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        var credentials = Encoding.UTF8
                                            .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                            .Split(':', 2);
                        if (credentials.Length == 2)
                        {
                            
                            if (UserService.GetUser(credentials[0], credentials[1]) is ApiUser user)
                            {
                                SetAuthroizedUserClaims(context, user);
                                return;
                            }                            
                        }
                    }
                }

                ReturnUnauthorizedResult(context);
            }
            catch (FormatException)
            {
                ReturnUnauthorizedResult(context);
            }
        }

        private void SetAuthroizedUserClaims(AuthorizationFilterContext context, ApiUser user)
        {
            context.HttpContext.SetClientGuid(user.Guid);
        }

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
        }
    }


}
