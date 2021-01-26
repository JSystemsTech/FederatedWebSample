using FederatedIPAuthenticationService.Services;
using ServiceLayer.DomainLayer;
using System;

namespace FederatedIPAuthenticationService.Attributes.Common
{

    public class FederatedProviderAttribute : FederatedAuthenticationProvider {
        private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        private IAuthenticationProviderDomainFacade AuthenticationProviderDomainFacade => Services.Get<IAuthenticationProviderDomainFacade>();

        protected override string GetSavedConsumerAuthenticationApiUrl(string key)
        {
            string authenticationRequestGuidStr = EncryptionService.DateSaltDecrypt(key);
            Guid authenticationRequestGuid = Guid.Parse(authenticationRequestGuidStr);
            return AuthenticationProviderDomainFacade.GetWebAuthenticationCache(authenticationRequestGuid);
        }
    }
    public class FederatedAllowAnnonomousAttribute : FederatedAuthenticationFilter
    {
        public FederatedAllowAnnonomousAttribute() : base(false) { }
    }
}