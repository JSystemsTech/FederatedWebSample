using FederatedIPAuthenticationService.Services;
using System;

namespace FederatedIPAuthenticationService.Attributes.Common
{
    /*Define Consuming application's Authentication attribute*/
    //public class FederatedConsumerAttribute : FederatedAuthenticationConsumer
    //{
    //    public FederatedConsumerAttribute(bool isAuthenticatedRoute = true) : base(isAuthenticatedRoute) { }
    //    private IUserManagmentService UserManagmentService => Services.Get<IUserManagmentService>();

    //    protected override IIdentity CreateAuthenticatedPrincipalIdentity(IDictionary<string, IEnumerable<string>> tokenClaims)
    //    {
    //        Guid? userGuid = tokenClaims.ParseFirst<Guid?>("UserGuid");
    //        if (userGuid is Guid guid)
    //        {
    //            /*Get User Info */
    //            // SomeUserModel user = GetSomeUserModel(userGuid);

    //            string firstName = "Demo";
    //            string middleInitial = "T";
    //            string lastName = "User";
    //            //var identity = ConsumerApplicationIdentity.Create(guid, firstName, middleInitial, lastName);
    //            return CommonIdentity.Create(guid, firstName, middleInitial, lastName);
    //        }
    //        else
    //        {
    //            throw new Exception("Token is missing UserGuid");
    //        }
    //    }
    //    protected override void SetTokenUpdateClaims(IIdentity identity, IDictionary<string, IEnumerable<string>> tokenClaims) {

    //        CommonIdentity userIdentity = identity as CommonIdentity;
    //    }
    //    protected override IEnumerable<string> GetRoles(IDictionary<string, IEnumerable<string>> tokenClaims, IEnumerable<string> currentRoles)
    //    {
    //        //tokenClaims.Get<string>("Roles");
    //        //Hardcoded Example Values for demo only
    //        IEnumerable<string> roles = new[] { "Role 1", "Role 2", "Role 3", "Role 4" };
    //        return roles;
    //    }

    //    protected override IEnumerable<ConsumerUser> ResolveTestUsers() => UserManagmentService.GetTestUsers();
    //}

    public class FederatedProviderAttribute : FederatedAuthenticationProvider {
        private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        private IAuthenticationRequestCache AuthenticationRequestCache { get => Services.Get<IAuthenticationRequestCache>(); }

        protected override string GetSavedAuthenticationRequest(string key)
        {
            string authenticationRequestGuidStr = EncryptionService.Decrypt(key);
            Guid authenticationRequestGuid = Guid.Parse(authenticationRequestGuidStr);
            return AuthenticationRequestCache.Get(authenticationRequestGuid);
        }
    }
    public class FederatedAllowAnnonomousAttribute : FederatedAuthenticationFilter
    {
        public FederatedAllowAnnonomousAttribute() : base(false) { }
    }
}