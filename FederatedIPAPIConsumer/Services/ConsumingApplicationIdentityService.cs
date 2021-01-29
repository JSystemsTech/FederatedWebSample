using FederatedAuthNAuthZ.Attributes.Common;
using FederatedAuthNAuthZ.Extensions;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using FederatedIPAuthenticationService.Services;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace FederatedIPAPIConsumer.Services
{
    public class ConsumingApplicationIdentityService : FederatedApplicationIdentityService
    {
        private IUserManagmentService UserManagmentService => Services.Get<IUserManagmentService>();
        public override IIdentity CreateAuthenticatedPrincipalIdentity(IDictionary<string, IEnumerable<string>> tokenClaims)
        {
            if (tokenClaims.ContainsKey("UserId") && tokenClaims["UserId"].First() is string userId && UserManagmentService.GetUser(userId) is ApplicationUser user)
            {
                var name = user.Name.Split(' ');

                string firstName = name[0];
                string middleInitial = name[1];
                string lastName = name[2];
                return CommonIdentity.Create(userId, firstName, middleInitial, lastName);
            }
            else
            {
                throw new Exception("Token is missing UserId");
            }
        }
        public override IEnumerable<string> GetRoles(IDictionary<string, IEnumerable<string>> tokenClaims, IEnumerable<string> currentRoles)
        {
            return tokenClaims.ContainsKey("UserId") && tokenClaims["UserId"].First() is string userId ? UserManagmentService.GetUserRoles(userId) : new string[0];
        }
    }
}