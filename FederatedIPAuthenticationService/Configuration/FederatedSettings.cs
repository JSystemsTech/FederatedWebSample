using System.Collections.Generic;
using System.Linq;

namespace FederatedIPAuthenticationService.Configuration
{

    public class FederatedSettings
    {
        internal static string FederatedAuthenticationRequestTokenCookiePrefix = "__FederatedAuthenticationRequestToken_";
        internal static string FederatedAuthenticationTokenCookiePrefix = "__FederatedAuthenticationToken_";
        internal static string ConsumerSettings = "federatedConsumerSettings";
        internal static string ProviderSettings = "federatedProviderSettings";
        internal static string TokenProviderSettings = "tokenProviderSettings";
        internal static string EmailProviderSettings = "emailProviderSettings";
        internal static string SiteMeta = "federatedSiteMeta";
        internal static string EncryptionServiceSettings = "encryptionServiceSettings";

        public static ISiteMeta GetSiteMeta(IDictionary<string, IEnumerable<string>> claims)=> new SiteMeta(claims.ToDictionary(i => i.Key, i => string.Join(",",i.Value)));
    }
    
}
