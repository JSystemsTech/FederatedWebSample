using FederatedAuthNAuthZ.Models;
using FederatedAuthNAuthZ.Services;
using FederatedIPAuthenticationService.Services;
using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FederatedIPAPIAuthenticationProviderWeb.Services
{
    public sealed class SelfContainedTokenProvider : Service, ITokenProvider
    {

        ITokenHandlerService TokenHandlerService => Services.Get<ITokenHandlerService>();
       
        private IEnumerable<TokenClaim> ToClaimsParams(IDictionary<string, IEnumerable<string>> claims) => claims.Select(c => new TokenClaim() { Name = c.Key, Values = c.Value });
        public string CreateToken(Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler)
        {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            tokenClaimHandler(claims);
            return TokenHandlerService.Create(ToClaimsParams(claims));
        }

        public string RenewToken(string token, Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler)
        {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            tokenClaimHandler(claims);
            return TokenHandlerService.Renew(token,ToClaimsParams(claims));
        }

        public IDictionary<string, IEnumerable<string>> GetTokenClaims(string token)
        {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            TokenHandlerService.GetClaims(token).ToList().ForEach(claim => claims.Add(claim.Name, claim.Values));
            return claims;
        }
        public DateTime? GetExpirationDate(string token)
        => TokenHandlerService.GetExpirationDate(token);
        public bool IsValidToken(string token) => TokenHandlerService.IsValid(token);
    }
}