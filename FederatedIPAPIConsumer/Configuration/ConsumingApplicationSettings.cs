using FederatedAuthNAuthZ.Configuration;
using ServiceLayer.DomainLayer.DbConnection;

namespace FederatedIPAPIConsumer.Configuration
{
    public class ConsumingApplicationSettings: FederatedApplicationSettingsConfig
    {
        private IDbStoredApplicationSettingsConfig Config => Services.Get<IDbStoredApplicationSettingsConfig>();
        public ConsumingApplicationSettings() : base() { IgnoreMissingConfigValues = true; }
        protected override void Init()
        {            
            base.Init();
            Config.InitSettings(SiteId, SiteEnvironment);
            string[] TokenProviderCredentials = Config.Get("TokenProviderCredentials").Split(';');
            TokenProviderUsername = TokenProviderCredentials[0];
            TokenProviderPassword = TokenProviderCredentials[1];
        }
    }
}