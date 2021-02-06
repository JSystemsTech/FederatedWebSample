using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Extensions;
using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using FederatedIPAPIAuthenticationProviderWeb.Models;
using Newtonsoft.Json;
using ServiceLayer.DomainLayer;
using ServiceLayer.DomainLayer.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace FederatedIPAPIAuthenticationProviderWeb.Services
{
    public class FederatedAuthenticationModeService : ProviderAuthenticationModeService
    {
        protected IFederatedApplicationSettings FederatedApplicationSettings => Services.Get<IFederatedApplicationSettings>();
        private IAuthenticationProviderDomainFacade AuthenticationProviderDomainFacade => Services.Get<IAuthenticationProviderDomainFacade>();
        private ITokenProvider TokenProvider => Services.Get<ITokenProvider>();
        protected override void RegisterAuthenticatonModes()
        {
            RegisterAuthenticationMethod("CAC", AuthenticateWithCACData);
            RegisterAuthenticationMethod("CommonCAC", AuthenticateWithCACDataCommon);
            RegisterAuthenticationMethod("Test", AuthenticateWithTestUser);

            RegisterRedirectMode("CAC", FederatedApplicationSettings.ExternalAuthenticationUrl);
            RegisterRedirectMode("CommonCAC", FederatedApplicationSettings.ExternalAuthenticationUrl);
            RegisterFormRedirectMode("CAC-Form", "CACForm",FederatedApplicationSettings.ExternalAuthenticationUrl);
            RegisterFormViewMode("Test", "TestUsersForm");
        }
        protected override IEnumerable<string> FilterAuthenticatonModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes)
        {
            return appApi.GetApplicationSettings().FederatedApplicationSettings.IsProductionEnvironment() ? 
                applicationRequestedModes.Where(m=> m !="Test"): 
                applicationRequestedModes;
        }
        private ICACUser GetCACUserData(string cacRequestToken) => AuthenticationProviderDomainFacade.GetCACLoginRequestUser(cacRequestToken);
        private ApplicationAuthenticationApiAuthenticationResponse AuthenticateWithCACData(IApplicationAuthenticationAPI appAPI, object values)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = null;
            if (values is string cacRequestToken && !string.IsNullOrWhiteSpace(cacRequestToken))
            {
                response = appAPI.Authenticate(new
                {
                    UserData = JsonConvert.SerializeObject(GetCACUserData(cacRequestToken))//presserialize before API serializes to keep it as a string
                });
            }
            return response;
        }
        private ApplicationAuthenticationApiAuthenticationResponse AuthenticateWithCACDataCommon(IApplicationAuthenticationAPI appAPI, object values)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = null;
            if (values is string cacRequestToken && !string.IsNullOrWhiteSpace(cacRequestToken))
            {
                ICACUser UserData = GetCACUserData(cacRequestToken);
                string authToken = TokenProvider.CreateToken(claims => {
                    appAPI.GetApplicationSettings().FederatedApplicationSettings.UpdateConsumerTokenClaims(claims, UserData.EDIPI);
                });
                response = new ApplicationAuthenticationApiAuthenticationResponse() { 
                    AuthenticationToken=authToken,
                    AuthenticationTokenExpiration= TokenProvider.GetExpirationDate(authToken),
                    Message= $"{UserData.FirstName} {UserData.MiddleInitial} {UserData.LastName}"
                };
            }
            return response;
        }
        private ApplicationAuthenticationApiAuthenticationResponse AuthenticateWithTestUser(IApplicationAuthenticationAPI appAPI, object values)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = null;
            if (values is TestUserVM model)
            {
                string authToken = TokenProvider.CreateToken(claims => {
                    claims.AddUpdate("TestUserId", model.TestUserId);
                    appAPI.GetApplicationSettings().FederatedApplicationSettings.UpdateConsumerTokenClaims(claims);
                });
                var user = appAPI.GetApplicationSettings().TestUsers.First(m => m.UserId == model.TestUserId);
                response = new ApplicationAuthenticationApiAuthenticationResponse()
                {
                    AuthenticationToken = authToken,
                    AuthenticationTokenExpiration = TokenProvider.GetExpirationDate(authToken),
                    Message = user.Name
                };
            }
            return response;
        }
        private ApplicationAuthenticationApiAuthenticationResponse AuthenticateWithUsernamePassword(IApplicationAuthenticationAPI appAPI, object values)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = null;
            if (values is UsernamePasswordVM model)
            {
                response = appAPI.Authenticate(new
                {
                    Username = model.Username,
                    Password = model.Password
                });
            }
            return response;
        }
        private ApplicationAuthenticationApiAuthenticationResponse AuthenticateWithEmailPassword(IApplicationAuthenticationAPI appAPI, object values)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = null;
            if (values is EmailPasswordVM model)
            {
                response = appAPI.Authenticate(new
                {
                    Email = model.Email,
                    Password = model.Password
                });
            }
            return response;
        }
    }
}