using FederatedAuthNAuthZ.Services;
using ServiceLayer.DomainLayer;
using ServiceProviderShared;
using System;

namespace FederatedAuthNAuthZ.Attributes.Common
{

    public class FederatedProviderAttribute : FederatedAuthenticationProvider {
        private IEncryptionService EncryptionService => ServiceManager.GetService<IEncryptionService>();
        private IAuthenticationProviderDomainFacade AuthenticationProviderDomainFacade => ServiceManager.GetService<IAuthenticationProviderDomainFacade>();
        public FederatedProviderAttribute(bool isAuthenticated = true) : base(isAuthenticated) { }
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