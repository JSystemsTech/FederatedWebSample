using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Models;
using FederatedIPAuthenticationService.Extensions;
using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiClient;

namespace FederatedAuthNAuthZ.Services
{
    public interface ITokenProvider
    {
        string CreateToken(Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler);

        string RenewToken(string token, Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler);

        IDictionary<string, IEnumerable<string>> GetTokenClaims(string token);
        DateTime? GetExpirationDate(string token);
        bool IsValidToken(string token);
    }
    public sealed class TokenProvider : Service,ITokenProvider
    {
        private IApiClient Client { get; set; }
        private IApiEndpoint RequestEndpoint { get; set; }
        private IApiEndpoint RenewEndpoint { get; set; }
        private IApiEndpoint ClaimsEndpoint { get; set; }
        private IApiEndpoint ExpirationEndpoint { get; set; }

        protected override void Init()
        {
            
            IFederatedApplicationSettings FederatedApplicationSettings = Services.Get<IFederatedApplicationSettings>();
            IEncryptionService encryptionService;
            string authenticationEndpointName = "Authentication".GetTokenProviderAPIEndpointName();
            if (FederatedApplicationSettings == null)
            {
                ITokenProviderSettings standAloneConfig = Services.Get<ITokenProviderSettings>();
                Client = ApiClientFactory.Create(standAloneConfig.Url, authenticationEndpointName, standAloneConfig.Username, standAloneConfig.Password);
                encryptionService = new FederatedApplicationBasicEncryptionService(standAloneConfig);
            }
            else
            {
                encryptionService = Services.Get<IEncryptionService>();
                Client = ApiClientFactory.Create(FederatedApplicationSettings.AuthenticationProviderUrl, authenticationEndpointName, FederatedApplicationSettings.TokenProviderUsername, FederatedApplicationSettings.TokenProviderPassword);                
            }
            Client.EncryptionHandler = str => encryptionService.DateSaltEncrypt(str);
            Client.DecryptionHandler = str => encryptionService.DateSaltDecrypt(str, true);

            RequestEndpoint = Client.CreateEndpoint("Request".GetTokenProviderAPIEndpointName());
            RenewEndpoint = Client.CreateEndpoint("Renew".GetTokenProviderAPIEndpointName());
            ClaimsEndpoint = Client.CreateEndpoint("Claims".GetTokenProviderAPIEndpointName());
            ExpirationEndpoint = Client.CreateEndpoint("Expires".GetTokenProviderAPIEndpointName());
        }
        private IEnumerable<TokenClaim> ToClaimsParams(IDictionary<string, IEnumerable<string>> claims) => claims.Select(c => new TokenClaim() { Name = c.Key, Values = c.Value });
        public string CreateToken(Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler) {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            tokenClaimHandler(claims);
            var response = RequestEndpoint.Post(new { Claims = ToClaimsParams(claims) });
            return response.Deserialize<string>(); 
        }

        public string RenewToken(string token, Action<IDictionary<string, IEnumerable<string>>> tokenClaimHandler)
        {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            tokenClaimHandler(claims);
            return RenewEndpoint.Post(new { Token = token , Claims = ToClaimsParams(claims) }).Deserialize<string>();
        }
        
        public IDictionary<string, IEnumerable<string>> GetTokenClaims(string token) {
            IDictionary<string, IEnumerable<string>> claims = new Dictionary<string, IEnumerable<string>>();
            ClaimsEndpoint.Post(new { Token = token }).Deserialize<TokenClaim[]>().ToList().ForEach(claim => claims.Add(claim.Name, claim.Values));
            return claims;
        }
        public DateTime? GetExpirationDate(string token)
        => ExpirationEndpoint.Post(new { Token = token }).Deserialize<DateTime?>();
        public bool IsValidToken(string token) => GetExpirationDate(token) is DateTime expirationDate && DateTime.UtcNow < expirationDate;
    }
}