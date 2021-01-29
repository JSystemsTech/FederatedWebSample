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
    public class ApplicationAuthenticationAPIController : ApplicationAuthenticationAPIControllerBase
    {
        private IUserManagmentService UserManagmentService => ServiceManager.GetService<IUserManagmentService>();
        private ApplicationUser ResolveUserWithBasicAuth(string username, string password)
        {
            if (username != null && password == "1234")
            {
                return UserManagmentService.ResolveUser(username, password);
            }
            return null;
        }
        private ApplicationUser ResolveUserWithEmailAuth(string email, string password)
        {
            if (email != null && password == "1234")
            {
                return UserManagmentService.ResolveUser(email, password);
            }
            return null;
        }
        
        protected override ApplicationUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials)
        {
            if (providerAuthenticationCredentials.TestUserId is string userId)
            {
                return UserManagmentService.GetTestUser(userId);
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

        protected override IEnumerable<ApplicationUser> GetTestUsers() => UserManagmentService.GetTestUsers();
        protected override string GetLogoImage() => LoadImageFromFile("~/Content/Images/logo.png");
    }
}