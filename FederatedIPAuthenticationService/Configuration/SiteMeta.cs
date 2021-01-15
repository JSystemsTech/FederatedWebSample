using ServiceProvider.Configuration;
using System;
using System.Collections.Generic;

namespace FederatedIPAuthenticationService.Configuration
{
    public interface ISiteMeta : IConfigurationSectionConfig
    {
        string SiteId { get; }
        string SiteRhealmId { get; }
        string SiteRhealm { get; }
        string SiteName { get; }
        string SiteVersion { get; }
        string SiteEnvironment { get; }
        string SiteDescription { get; }        
        string ConsumerApiUrlBase { get; }
        IEnumerable<string> AuthenticationModes { get; }
    }

    internal class SiteMeta : ConfigurationSectionConfig, ISiteMeta
    {
        protected override string ConfiguationSection => FederatedSettings.SiteMeta;
        public string SiteId { get; set; }
        public string SiteRhealmId { get; set; }
        public string SiteRhealm { get; set; }
        public string SiteName { get; set; }
        public string SiteVersion { get; set; }
        public string SiteEnvironment { get; set; }
        public string SiteDescription { get; set; }
        public string ConsumerApiUrlBase { get; set; }
        public IEnumerable<string> AuthenticationModes { get; set; }
        public SiteMeta() : base() { }
        public SiteMeta(IDictionary<string, string> collection) : base() { Collection = collection; Init(); }
        
        protected override void Init()
        {
            SiteId = GetValue("SiteId", true);
            SiteRhealmId = GetValue("SiteRhealmId");
            SiteRhealm = GetValue("SiteRhealm");
            SiteName = GetValue("SiteName", true);
            SiteVersion = GetValue("SiteVersion", true);
            SiteEnvironment = GetValue("SiteEnvironment", true);
            SiteDescription = GetValue("SiteDescription", true);

            ConsumerApiUrlBase = GetValue("ConsumerApiUrlBase");
            AuthenticationModes = GetValue("AuthenticationModes").Split(',');
        }
    }
}
