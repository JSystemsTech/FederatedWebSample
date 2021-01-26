using System.Collections.Generic;
using System.Linq;

namespace FederatedIPAuthenticationService.Configuration
{

    public class FederatedSettings
    {
        internal static string FederatedAuthenticationRequestTokenCookiePrefix = "__FederatedAuthenticationRequestToken_";
        internal static string FederatedAuthenticationTokenCookiePrefix = "__FederatedAuthenticationToken_";
        internal static string TokenProviderSettings = "tokenProviderSettings";
        internal static string EmailProviderSettings = "emailProviderSettings";
        
        internal static string EncryptionServiceSettings = "encryptionServiceSettings";
    }
    
}
