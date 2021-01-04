﻿using ServiceLayer.Services;
using FederatedIPAuthenticationService.Attributes.Common;
using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Models;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FederatedIPAPIConsumer.Controllers
{
    public class ProviderAPIController : ConsumerAPIController
    {
        private IUserManagmentService UserManagmentService => Services.Get<IUserManagmentService>();
        
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
        protected override IEnumerable<ConsumerUser> ResolveTestUsers() => UserManagmentService.GetTestUsers();
        protected override ConsumerUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials, IEnumerable<string> externalAuthAuthorizedUser)
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
            else if (externalAuthAuthorizedUser != null)
            {
                CACAuthorizedUser user = new CACAuthorizedUser(externalAuthAuthorizedUser);
                return UserManagmentService.ResolveUser(user.EDIPI);
            }
            return null;
        }
        
    }
}