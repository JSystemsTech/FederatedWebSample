using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace FederatedIPAuthenticationService.Principal
{
    public class FederatedPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public IEnumerable<string> Roles { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public double SessionTimeout { get => ExpirationDate is DateTime expirationDate && DateTime.UtcNow < expirationDate ? (expirationDate - DateTime.UtcNow).TotalMilliseconds : 0; }
        public bool IsInRole(string role) => Roles != null && Roles.Contains(role);

        private FederatedPrincipal() { }
        public static FederatedPrincipal Create(IEnumerable<string> roles, DateTime? expirationDate, IIdentity identity) 
            => new FederatedPrincipal() { Roles = roles, ExpirationDate = expirationDate, Identity = identity };
        public static FederatedPrincipal CreateLogout() => new FederatedPrincipal() { Roles = new string[0], Identity = null };
        internal TIdentity GetIdentity<TIdentity>()
            where TIdentity : IIdentity
            => Identity is TIdentity identity ? identity: default;
    }
}