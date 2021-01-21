using ServiceProvider.Configuration;

namespace FederatedIPAuthenticationService.Configuration
{
    public interface IFederatedConsumerSettings : IConfigurationSectionConfig
    {
        bool UseSessionCookie { get; }        
        string LogoutUrl { get; }
        string AuthenticationProviderId { get; }
        string AuthenticationProviderUrl { get; }       
        string RedirectUrl { get; }
        string TokenProviderAuthenticationEndpoint { get; }
        string TokenProviderUsername { get; }
        string TokenProviderPassword { get; }
    }

    public sealed class FederatedConsumerSettings : ConfigurationSectionConfig, IFederatedConsumerSettings
    {
        protected override string ConfiguationSection => FederatedSettings.ConsumerSettings;
        public bool UseSessionCookie { get; set; }
        public string LogoutUrl { get; set; }
        public string AuthenticationProviderId { get; set; }
        public string AuthenticationProviderUrl { get; set; }
        public string RedirectUrl { get; set; }

        public string TokenProviderAuthenticationEndpoint { get; set; }
        public string TokenProviderUsername { get; set; }
        public string TokenProviderPassword { get; set; }
        public FederatedConsumerSettings() : base() { }
        protected override void Init()
        {
            UseSessionCookie = bool.Parse(GetValue("UseSessionCookie"));
            LogoutUrl = GetValue("LogoutUrl", true);
            AuthenticationProviderId = GetValue("AuthenticationProviderId", true);
            AuthenticationProviderUrl = GetValue("AuthenticationProviderUrl",true);
            RedirectUrl = GetValue("RedirectUrl", true);
            TokenProviderAuthenticationEndpoint = GetValue("TokenProviderAuthenticationEndpoint", true);
            TokenProviderUsername = GetValue("TokenProviderUsername", true);
            TokenProviderPassword = GetValue("TokenProviderPassword", true);
        }
    }

}
