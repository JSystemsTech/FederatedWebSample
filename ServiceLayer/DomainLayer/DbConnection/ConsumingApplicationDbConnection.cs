using DbFacade.DataLayer.ConnectionService;
using DbFacade.Services;
using System.Configuration;
using System.Threading.Tasks;

namespace ServiceLayer.DomainLayer.DbConnection
{
    internal class ConsumingApplicationDbConnection: SqlConnectionConfig<ConsumingApplicationDbConnection>
    {
        private ConsumingApplicationDbConnection(ConnectionStringSettings connectionStringSettings) { ConnectionStringSettings = connectionStringSettings; }
        private  ConnectionStringSettings ConnectionStringSettings { get; set; }
        protected override string GetDbConnectionString() => ConnectionStringSettings.ConnectionString;
        protected override string GetDbConnectionProvider() => ConnectionStringSettings.ProviderName;

        protected override async Task<string> GetDbConnectionStringAsync()
        {
            await Task.CompletedTask;
            return ConnectionStringSettings.ConnectionString;
        }

        protected override async Task<string> GetDbConnectionProviderAsync()
        {
            await Task.CompletedTask;
            return ConnectionStringSettings.ProviderName;
        }
        public static void RegisterConnection(ConnectionStringSettings connectionStringSettings) => DbConnectionService.Register(new ConsumingApplicationDbConnection(connectionStringSettings));
        public static IDbCommandConfig GetTestUsers = CreateFetchCommand("[dbo].[GetTestUsers]", "Get Test Users");
        
    }
    internal class ApplicationSettingsDbConnection : SqlConnectionConfig<ApplicationSettingsDbConnection>
    {
        private ApplicationSettingsDbConnection(ConnectionStringSettings connectionStringSettings) { ConnectionStringSettings = connectionStringSettings; }
        private ConnectionStringSettings ConnectionStringSettings { get; set; }
        protected override string GetDbConnectionString() => ConnectionStringSettings.ConnectionString;
        protected override string GetDbConnectionProvider() => ConnectionStringSettings.ProviderName;

        protected override async Task<string> GetDbConnectionStringAsync()
        {
            await Task.CompletedTask;
            return ConnectionStringSettings.ConnectionString;
        }

        protected override async Task<string> GetDbConnectionProviderAsync()
        {
            await Task.CompletedTask;
            return ConnectionStringSettings.ProviderName;
        }
        public static void RegisterConnection(ConnectionStringSettings connectionStringSettings) => DbConnectionService.Register(new ApplicationSettingsDbConnection(connectionStringSettings));
        public static IDbCommandConfig GetApplicationSettings = CreateFetchCommand("[dbo].[GetApplicationSettings]", "Get Application Settings");

    }

}
