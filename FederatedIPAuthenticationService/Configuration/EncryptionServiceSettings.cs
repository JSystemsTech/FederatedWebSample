using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Configuration
{
    public interface IEncryptionServiceSettings : IConfigurationSectionConfig
    {
        string Key { get; }
    }
    public sealed class EncryptionServiceSettings : ConfigurationSectionConfig, IEncryptionServiceSettings
    {
        protected override string ConfiguationSection => FederatedSettings.EncryptionServiceSettings;
        public string Key { get; set; }
        public EncryptionServiceSettings() : base() { }
        protected override void Init()
        {
            Key = GetValue("Key");
        }
    }
}
