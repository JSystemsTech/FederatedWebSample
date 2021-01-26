using ServiceProvider.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Configuration
{
    public interface ITokenProviderSettings : IConfigurationSectionConfig
    {
        string Url { get; }
        string AuthenticationEndpoint { get; }
        string ProviderNetwork { get; }
        string ProviderId { get; }
        string Username { get; }
        string Password { get; }
    }
    public sealed class TokenProviderSettings : ConfigurationSectionConfig, ITokenProviderSettings
    {
        protected override string ConfiguationSection => FederatedSettings.TokenProviderSettings;
        public string Url { get; set; }
        public string AuthenticationEndpoint { get; set; }
        public string ProviderNetwork { get; set; }
        public string ProviderId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public TokenProviderSettings() : base() { }
        protected override void Init() {
            Url = GetValue("Url", true);
            AuthenticationEndpoint = GetValue("AuthenticationEndpoint", true);
            ProviderNetwork = GetValue("ProviderNetwork", true);
            ProviderId = GetValue("ProviderId", true);
            Username = GetValue("Username", true);
            Password = GetValue("Password", true);
        }
    }
}
