using ServiceProvider.Configuration;
using ServiceProvider.Services;

namespace ServiceLayer.DomainLayer.DbConnection
{
    public class DbConnectionConfigService : Service
    {
        protected override void Init()
        {
            ConsumingApplicationDbConnection.RegisterConnection(Services.GetConnectionString("ConsumingApplicationDb"));
        }
    }
    public class AuthenticationProviderDbConnectionConfigService : Service
    {
        protected override void Init()
        {
            AuthenticationProviderDbConnection.RegisterConnection(Services.GetConnectionString("AuthenticationProviderDb"));
        }
    }
}
