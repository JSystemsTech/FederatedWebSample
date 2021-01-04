using FederatedIPAuthenticationService.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace FederatedIPAuthenticationService.Configuration
{
    public interface IConnectionStringConfig : IConfigurationSectionConfig
    {
        ConnectionStringSettings GetConnectionString(string key);
    }
    public class ConnectionStringConfig: ConfigurationSectionConfig, IConnectionStringConfig, ISpecialConfiguration
    {
        private IDictionary<string, ConnectionStringSettings> ConnectionStrings { get; set; }
        protected static ConnectionStringSettings CreateConnectionStringSettings<T>(T model, Func<T, string> name, Func<T, string> connectionString, Func<T, string> providerName)
        => new ConnectionStringSettings(name(model), connectionString(model), providerName(model));
        protected static ConnectionStringSettings CreateConnectionStringSettings<T>(T model, Func<T, string> name, Func<T, string> connectionString)
        => new ConnectionStringSettings(name(model), connectionString(model));
        protected static ConnectionStringSettings CreateConnectionStringSettings(string name, string connectionString, string providerName)
        => new ConnectionStringSettings(name, connectionString, providerName);
        protected static ConnectionStringSettings CreateConnectionStringSettings(string name, string connectionString)
        => new ConnectionStringSettings(name, connectionString);
        protected override IDictionary<string, string> GetConfigurationSource()
        {
            foreach(ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings)
            {
                ConnectionStrings.Add(settings.Name, settings);
            }
            return new Dictionary<string, string> ();
        }

        public ConnectionStringSettings GetConnectionString(string key) => ConnectionStrings.ContainsKey(key) ? ConnectionStrings[key] : null;

        public ConnectionStringConfig():base(){ ConnectionStrings = new Dictionary<string, ConnectionStringSettings>(); }
    }
}
