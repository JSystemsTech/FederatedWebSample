using ServiceProvider.Configuration;
using ServiceProvider.Services;

namespace ServiceLayer.DomainLayer.DbConnection
{
    public class DbConnectionConfigService : Service
    {
        private static string ConsumingApplicationDb = "ConsumingApplicationDb";
        private IConnectionStringConfig ConnectionStringConfig { get; set; }
        protected override void Init()
        {
            ConnectionStringConfig = Services.Get<IConnectionStringConfig>();
            ConsumingApplicationDbConnection.RegisterConnection(ConnectionStringConfig.GetConnectionString(ConsumingApplicationDb));
        }
    }
    public class AuthenticationProviderDbConnectionConfigService : Service
    {
        private static string ConsumingApplicationDb = "AuthenticationProviderDb";
        private IConnectionStringConfig ConnectionStringConfig { get; set; }
        protected override void Init()
        {
            ConnectionStringConfig = Services.Get<IConnectionStringConfig>();
            AuthenticationProviderDbConnection.RegisterConnection(ConnectionStringConfig.GetConnectionString(ConsumingApplicationDb));
        }
    }
}
