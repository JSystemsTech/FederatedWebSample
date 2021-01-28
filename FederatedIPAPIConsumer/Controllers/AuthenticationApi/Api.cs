using FederatedAuthNAuthZ.Extensions;
using FederatedAuthNAuthZ.Models;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using Newtonsoft.Json;
using ServiceLayer.DomainLayer.Models.Data;
using ServiceLayer.Services;
using ServiceProviderShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FederatedIPAPIConsumer.Controllers.AuthenticationApi
{
    public class AuthenticationController : ConsumerAuthenticationApiAuthenticationController
    {
        private IUserManagmentService UserManagmentService => ServiceManager.GetService<IUserManagmentService>();
        private ConsumerUser ResolveUserWithBasicAuth(string username, string password)
        {
            if (username != null && password == "1234")
            {
                return UserManagmentService.ResolveUser(username, password);
            }
            return null;
        }
        private ConsumerUser ResolveUserWithEmailAuth(string email, string password)
        {
            if (email != null && password == "1234")
            {
                return UserManagmentService.ResolveUser(email, password);
            }
            return null;
        }
        
        protected override ConsumerUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials)
        {
            if (providerAuthenticationCredentials.TestUserGuid is Guid userGuid)
            {
                return UserManagmentService.GetTestUser(userGuid);
            }
            else if (!string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Username) && !string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Password))
            {
                return ResolveUserWithBasicAuth(providerAuthenticationCredentials.Username, providerAuthenticationCredentials.Password);
            }
            else if (!string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Email) && !string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Password))
            {
                return ResolveUserWithEmailAuth(providerAuthenticationCredentials.Email, providerAuthenticationCredentials.Password);
            }
            else if (providerAuthenticationCredentials.UserData != null)
            {
                if(providerAuthenticationCredentials.UserData.TryDeserializeObject(out CACUser user)){
                    return UserManagmentService.ResolveUser(user.EDIPI);
                }
            }
            return null;
        }
    }
    public class ConsumerApplicationSettingsController : ConsumerAuthenticationApiConsumerApplicationSettingsController
    {
        private IUserManagmentService UserManagmentService => ServiceManager.GetService<IUserManagmentService>();
        protected override IEnumerable<ConsumerUser> GetTestUsers() => UserManagmentService.GetTestUsers();
        protected override string GetLogoImage() => LoadImageFromFile("~/Content/Images/logo.png");
    }



}