using ServiceProvider.Configuration;
using System;

namespace FederatedIPAPIAuthenticationProviderWeb.Configuration
{
    public interface ITokenProviderServiceSettings : IConfigurationSectionConfig
    {
        Uri AudienceUri { get; }
        string ConfirmationMethod { get; }
        string Issuer { get; }
        string Namespace { get; }
        string SubjectName { get; }
        int ValidFor { get; }
    }
    public sealed class TokenProviderServiceSettings : ConfigurationSectionConfig, ITokenProviderServiceSettings
    {
        
        protected override string ConfiguationSection => "tokenProviderServiceSettings";
        public Uri AudienceUri { get; set; }
        public string ConfirmationMethod { get; set; }
        public string Issuer { get; set; }
        public string Namespace { get; set; }
        public string SubjectName { get; set; }
        public int ValidFor { get; set; }
        public TokenProviderServiceSettings() : base() { }
        protected override void Init()
        {
            AudienceUri = new Uri(GetValue("AudienceUri", true));
            ConfirmationMethod = GetValue("ConfirmationMethod", true);
            Issuer = GetValue("Issuer", true);
            Namespace = GetValue("Namespace", true);
            SubjectName = GetValue("SubjectName", true);
            ValidFor = int.Parse(GetValue("ValidFor", true));
        }
    }
}