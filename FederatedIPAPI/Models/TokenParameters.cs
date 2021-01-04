using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FederatedIPAPI.Models
{
    public class TokenParameters
    {
        public string Token { get; set; }
        public string Value { get; set; }
        public IEnumerable<string> Values { get; set; }
        public IEnumerable<TokenClaim> Claims { get; set; }
        public TokenClaim Claim { get; set; }
        internal IEnumerable<string> GetValues() => Values != null ? Values : !string.IsNullOrWhiteSpace(Value) ? Value.Split(",") : new string[0];
        internal IEnumerable<TokenClaim> GetClaims() => Claims != null ? Claims : Claim != null ? new TokenClaim[1] { Claim } : new TokenClaim[0];
    }
}
