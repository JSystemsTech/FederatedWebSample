using ServiceProvider.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Services
{
    public interface IAuthenticationRequestCache
    {
        Guid Add(string token);
        string Get(Guid key);
    }
    public sealed class AuthenticationRequestCache : Service, IAuthenticationRequestCache
    {
        /*Use Thread Safe Dictionary*/
        private ConcurrentDictionary<Guid, string> RequestTokens { get; set; }
        public AuthenticationRequestCache() : base() { }
        protected override void Init()
        {
            RequestTokens = new ConcurrentDictionary<Guid, string>();
        }
        public Guid Add(string token)
        {
            Guid key = Guid.NewGuid();
            if(RequestTokens.TryAdd(key, token))
            {
                return key;
            }
            else
            {
                throw new Exception("Cannot save token");
            }
            
        }
        public string Get(Guid key)
        {
            if (RequestTokens.TryGetValue(key, out string token))
            {
                RequestTokens.TryRemove(key, out string removedToken);
                return token;
            }
            else
            {
                throw new Exception("Cannot find token");
            }

        }
    }
}
