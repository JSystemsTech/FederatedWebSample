using FederatedAuthNAuthZ.Models;
using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedAuthNAuthZ.Services
{
    public interface ITokenHandlerService
    {
        string Create(IEnumerable<TokenClaim> claims);
        string Renew(string tokenStr, IEnumerable<TokenClaim> claims);
        string Renew(string tokenStr, TokenClaim claim);
        IEnumerable<TokenClaim> GetClaims(string tokenStr);
        DateTime? GetExpirationDate(string tokenStr);
        bool IsValid(string tokenStr);
    }
    public abstract class TokenHandlerServiceBase : Service, ITokenHandlerService
    {
        public abstract string Create(IEnumerable<TokenClaim> claims);

        public abstract IEnumerable<TokenClaim> GetClaims(string tokenStr);

        public abstract DateTime? GetExpirationDate(string tokenStr);

        public abstract bool IsValid(string tokenStr);

        public abstract string Renew(string tokenStr, IEnumerable<TokenClaim> claims);

        public abstract string Renew(string tokenStr, TokenClaim claim);
    }
}
