using FederatedAuthNAuthZ.Services;
using ServiceLayer.DomainLayer;
using ServiceLayer.DomainLayer.Models.Data;
using ServiceProviderShared;

namespace FederatedIPAPIAuthenticationProviderWeb.Services
{
    public class TokenProviderAPIAuthenticationService: APIAuthenticationServiceBase
    {
        private IAuthenticationProviderDomainFacade AuthenticationProviderDomainFacade => ServiceManager.GetService<IAuthenticationProviderDomainFacade>();

        public override string AuthenticationApiUser(string username, string password)
        => AuthenticationProviderDomainFacade.AuthenticationApiUser(username, password) is AuthenticationProviderApiUser user ? user.Name : null;
    }
}