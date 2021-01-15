using FederatedIPAuthenticationService.Configuration;
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
}
