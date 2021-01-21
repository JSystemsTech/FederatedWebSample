using FederatedIPAuthenticationService.Services;
using System;

namespace FederatedIPAuthenticationService.Attributes.Common
{

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