
using FederatedIPAuthenticationService.Attributes;
using FederatedIPAuthenticationService.Attributes.Common;
using FederatedIPAuthenticationService.Extensions;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace ServiceLayer.AuthNAuthZ
{
    /*Define Consuming application's Authentication attribute*/
    public class FederatedAuthorizeAttribute : FederatedAuthenticationConsumer
    {
        private IUserManagmentService UserManagmentService => Services.Get<IUserManagmentService>();
        public FederatedAuthorizeAttribute(bool isAuthenticatedRoute = true) : base(isAuthenticatedRoute) { }

        protected override IIdentity CreateAuthenticatedPrincipalIdentity(IDictionary<string, IEnumerable<string>> tokenClaims)
        {
            Guid? userGuid = tokenClaims.ParseFirst<Guid?>("UserGuid");
            if (userGuid is Guid guid)
            {
                var user = UserManagmentService.GetUser(guid);
                var name = user.Name.Split(' ');

                string firstName = name[0];
                string middleInitial = name[1];
                string lastName = name[2];
                return CommonIdentity.Create(guid, firstName, middleInitial, lastName);
            }
            else
            {
                throw new Exception("Token is missing UserGuid");
            }
        }
        protected override void SetTokenUpdateClaims(IIdentity identity, IDictionary<string, IEnumerable<string>> tokenClaims) {

            CommonIdentity userIdentity = identity as CommonIdentity;
        }
        protected override IEnumerable<string> GetRoles(IDictionary<string, IEnumerable<string>> tokenClaims, IEnumerable<string> currentRoles)
        {
            return tokenClaims.ParseFirst<Guid?>("UserGuid") is Guid guid ? UserManagmentService.GetUserRoles(guid) : new string[0];
        }
            
    }
}