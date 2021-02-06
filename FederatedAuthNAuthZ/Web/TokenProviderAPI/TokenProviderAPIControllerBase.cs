using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Models;
using FederatedAuthNAuthZ.Services;
using Newtonsoft.Json;
using ServiceProviderShared;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FederatedAuthNAuthZ.Web.TokenProviderAPI
{

    public class TokenParameters
    {
        public string Token { get; set; }
        public string Value { get; set; }
        public IEnumerable<string> Values { get; set; }
        public IEnumerable<TokenClaim> Claims { get; set; }
        public TokenClaim Claim { get; set; }
        internal IEnumerable<string> GetValues() => Values != null ? Values : !string.IsNullOrWhiteSpace(Value) ? Value.Split(',') : new string[0];
        internal IEnumerable<TokenClaim> GetClaims() => Claims != null ? Claims : Claim != null ? new TokenClaim[1] { Claim } : new TokenClaim[0];
        internal IDictionary<string, IEnumerable<string>> ToClaimsDictionary() => GetClaims().ToDictionary(claim => claim.Name, claim => claim.GetValues());
    }
    [TokenProviderAPIAuthentication]
    public abstract class TokenProviderAPIControllerBase : ApiControllerBase
    {
        private ITokenHandlerService TokenHandlerService => ServiceManager.GetService<ITokenHandlerService>();
        private IFederatedApplicationSettings FederatedApplicationSettings => ServiceManager.GetService<IFederatedApplicationSettings>();
        private ITokenProvider TokenProvider => ServiceManager.GetService<ITokenProvider>();


        protected TokenParameters GetBodyAsTokenParameters(EncryptedPostBody body) => JsonConvert.DeserializeObject<TokenParameters>(body.Content.Decrypt());

        protected HttpResponseMessage CreateToken(EncryptedPostBody body) => EncryptedResponse(TokenHandlerService.Create(GetBodyAsTokenParameters(body).GetClaims()));
        protected HttpResponseMessage GetClaims(EncryptedPostBody body) => EncryptedResponse(TokenHandlerService.GetClaims(GetBodyAsTokenParameters(body).Token));
        protected HttpResponseMessage RenewToken(EncryptedPostBody body)
        {
            TokenParameters model = GetBodyAsTokenParameters(body);
            return EncryptedResponse(TokenHandlerService.Renew(model.Token, model.GetClaims()));
        }
        protected HttpResponseMessage GetExpirationDate(EncryptedPostBody body) => EncryptedResponse(TokenHandlerService.GetExpirationDate(GetBodyAsTokenParameters(body).Token));
        protected HttpResponseMessage BuildApiAuthToken() => EncryptedResponse(TokenProvider.CreateToken(claims => {
            claims.Add($"{FederatedApplicationSettings.SiteId}Name", new string[] { HttpContext.Current.User.Identity.Name.Encrypt() });
        }));


        [TokenProviderAPIAuthentication(true)]
        [HttpPost]
        public HttpResponseMessage Authentication() => BuildApiAuthToken();

        [HttpPost]
        [ActionName("Request")]
        public HttpResponseMessage RequestToken([FromBody] EncryptedPostBody body) => CreateToken(body);

        [HttpPost]
        public HttpResponseMessage Claims([FromBody] EncryptedPostBody body) => GetClaims(body);

        [HttpPost]
        public HttpResponseMessage Renew([FromBody] EncryptedPostBody body) => RenewToken(body);
        [HttpPost]
        public HttpResponseMessage Expires([FromBody] EncryptedPostBody body) => GetExpirationDate(body);
    }
}
