using DbFacade.DataLayer.ConnectionService;
using DbFacade.Services;
using System.Configuration;
using System.Threading.Tasks;

namespace ServiceLayer.DomainLayer.DbConnection
{
    internal class AuthenticationProviderDbConnection : SqlConnectionConfig<AuthenticationProviderDbConnection>
    {
        private AuthenticationProviderDbConnection(ConnectionStringSettings connectionStringSettings) { ConnectionStringSettings = connectionStringSettings; }
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
        public static void RegisterConnection(ConnectionStringSettings connectionStringSettings) => DbConnectionService.Register(new AuthenticationProviderDbConnection(connectionStringSettings));
        public static IDbCommandConfig AuthenticateApiUser = CreateFetchCommand("[dbo].[AuthenticateApiUser]", "Authenticate Api User");
        public static IDbCommandConfig AddWebAuthenticationCache = CreateTransactionCommand("[dbo].[WebAuthenticationCache_Add]", "Add Web Authentication Cache");
        public static IDbCommandConfig GetWebAuthenticationCache = CreateFetchCommand("[dbo].[WebAuthenticationCache_Get]", "Get Web Authentication Cache");
        public static IDbCommandConfig GetCACLoginRequestUser = CreateFetchCommand("[dbo].[GetCACLoginRequestUser]", "Get CAC Login request user");
        
    }
}
