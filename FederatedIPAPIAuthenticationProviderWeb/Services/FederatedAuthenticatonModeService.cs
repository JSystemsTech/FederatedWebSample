using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using FederatedIPAPIAuthenticationProviderWeb.Models;
using FederatedAuthNAuthZ.Services;
using Newtonsoft.Json;
using ServiceLayer.DomainLayer;
using ServiceLayer.DomainLayer.Models.Data;
using ServiceProvider.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FederatedIPAPIAuthenticationProviderWeb.Services
{
    public class FederatedAuthenticationModeService : ProviderAuthenticationModeService
    {
        protected IFederatedApplicationSettings FederatedApplicationSettings => Services.Get<IFederatedApplicationSettings>();
        private IAuthenticationProviderDomainFacade AuthenticationProviderDomainFacade => Services.Get<IAuthenticationProviderDomainFacade>();
        protected override void RegisterAuthenticatonModes()
        {
            RegisterAuthenticationMethod("CAC", AuthenticateWithCACData);
            RegisterAuthenticationMethod("Test", AuthenticateWithTestUser);

            RegisterRedirectMode("CAC", FederatedApplicationSettings.ExternalAuthenticationUrl);
            RegisterFormRedirectMode("CAC-Form", "CACForm",FederatedApplicationSettings.ExternalAuthenticationUrl);
            RegisterFormViewMode("Test", "TestUsersForm");
        }
        protected override IEnumerable<string> FilterAuthenticatonModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes)
        {
            return appApi.GetApplicationSettings().FederatedApplicationSettings.IsProductionEnvironment() ? 
                applicationRequestedModes.Where(m=> m !="Test"): 
                applicationRequestedModes;
        }
        private ApplicationAuthenticationApiAuthenticationResponse AuthenticateWithCACData(IApplicationAuthenticationAPI appAPI, object values)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = null;
            if (values is string token && !string.IsNullOrWhiteSpace(token))
            {
                ICACUser UserData = AuthenticationProviderDomainFacade.GetCACLoginRequestUser(token);
                response = appAPI.Authenticate(new
                {
                    UserData = JsonConvert.SerializeObject(UserData)//presserialize before API serializes to keep it as a string
                });
            }
            return response;
        }
        private ApplicationAuthenticationApiAuthenticationResponse AuthenticateWithTestUser(IApplicationAuthenticationAPI appAPI, object values)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = null;
            if (values is TestUserVM model)
            {
                response = appAPI.Authenticate(new
                {
                    TestUserId = model.TestUserId
                });
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