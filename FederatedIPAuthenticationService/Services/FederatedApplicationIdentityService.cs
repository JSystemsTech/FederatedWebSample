using ServiceProvider.Services;
using System.Collections.Generic;
using System.Security.Principal;

namespace FederatedAuthNAuthZ.Services
{
    public interface IFederatedApplicationIdentityService
    {
        IIdentity CreateAuthenticatedPrincipalIdentity(IDictionary<string, IEnumerable<string>> tokenClaims);
        void SetTokenUpdateClaims(IIdentity identity, IDictionary<string, IEnumerable<string>> tokenClaims);
        IEnumerable<string> GetRoles(IDictionary<string, IEnumerable<string>> tokenClaims, IEnumerable<string> currentRoles);
        void OnLogout(IIdentity identity);
    }
    public abstract class FederatedApplicationIdentityService: Service, IFederatedApplicationIdentityService
    {
        public abstract IIdentity CreateAuthenticatedPrincipalIdentity(IDictionary<string, IEnumerable<string>> tokenClaims);
        public virtual void SetTokenUpdateClaims(IIdentity identity, IDictionary<string, IEnumerable<string>> tokenClaims) { }
        public abstract IEnumerable<string> GetRoles(IDictionary<string, IEnumerable<string>> tokenClaims, IEnumerable<string> currentRoles);
        public virtual void OnLogout(IIdentity identity) { }
    }
}
