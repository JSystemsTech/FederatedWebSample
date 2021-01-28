using FederatedAuthNAuthZ.Services;
using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FederatedIPAPIAuthenticationProviderWeb.Services
{
    public sealed class SelfContainedTokenProvider : Service, ITokenProvider
    {

        ITokenProviderService TokenProviderService => Services.Get<ITokenProviderService>();
       
        private IEnumerable<TokenClaim> ToClaimsParams(IDictionary<string, IEnumerable<string>> claims) => claims.Select(c => new TokenClaim() { Name = c.Key, Values = c.Value });
        public string CreateToken(Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler)
        {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            tokenClaimHandler(claims);
            return TokenProviderService.Create(ToClaimsParams(claims));
        }

        public string RenewToken(string token, Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler)
        {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            tokenClaimHandler(claims);
            return TokenProviderService.Renew(token,ToClaimsParams(claims));
        }

        public IDictionary<string, IEnumerable<string>> GetTokenClaims(string token)
        {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            TokenProviderService.GetClaims(token).ToList().ForEach(claim => claims.Add(claim.Name, claim.Values));
            return claims;
        }
        public DateTime? GetExpirationDate(string token)
        => TokenProviderService.GetExpirationDate(token);
        public bool IsValidToken(string token) => TokenProviderService.IsValid(token);
    }
}