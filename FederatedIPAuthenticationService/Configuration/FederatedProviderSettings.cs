using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Configuration
{
    public interface IFederatedProviderSettings : IConfigurationSectionConfig
    {
        string AuthenticationRequestUrl { get; }
        string ExternalAuthenticationtUrl { get; }
        string ExternalAuthenticationPostbackRequestUrl { get; }
        string LoginPostRequestUrl { get; }
        string DefaultConsumerUrl { get; }


    }

    public sealed class FederatedProviderSettings : ConfigurationSectionConfig, IFederatedProviderSettings
    {
        protected override string ConfiguationSection => FederatedSettings.ProviderSettings;
        public string AuthenticationRequestUrl { get; set; }
        public string ExternalAuthenticationtUrl { get; set; }
        public string ExternalAuthenticationPostbackRequestUrl { get; set; }
        public string LoginPostRequestUrl { get; set; }
        public string DefaultConsumerUrl { get; set; }
        public FederatedProviderSettings() : base() { }
        protected override void Init()
        {
            AuthenticationRequestUrl = GetValue("AuthenticationRequestUrl", true);
            ExternalAuthenticationtUrl = GetValue("ExternalAuthenticationtUrl", true);
            ExternalAuthenticationPostbackRequestUrl = GetValue("ExternalAuthenticationPostbackRequestUrl", true);
            LoginPostRequestUrl = GetValue("LoginPostRequestUrl", true);
            DefaultConsumerUrl = GetValue("DefaultConsumerUrl");
        }
    }
}
