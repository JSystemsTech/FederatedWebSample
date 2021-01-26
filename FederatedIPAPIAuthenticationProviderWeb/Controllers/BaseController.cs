using FederatedIPAuthenticationService.Services;
using ServiceLayer.DomainLayer;
using System;
using WebApiClientShared.Web;

namespace FederatedIPAPIAuthenticationProviderWeb.Controllers
{
    public abstract class BaseController: FederatedProviderWebController
    {
        protected IAuthenticationProviderDomainFacade AuthenticationProviderDomainFacade => Services.Get<IAuthenticationProviderDomainFacade>();
        private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        protected override string SaveAuthenticationRequest(string authenticationRequestToken)
        {
            Guid authenticationRequestGuid = AuthenticationProviderDomainFacade.AddWebAuthenticationCache(ConsumingApplicationFederatedApplicationSettings.ConsumerAuthenticationApiUrl);
            return EncryptionService.DateSaltEncrypt(authenticationRequestGuid.ToString());
        }
    }
}