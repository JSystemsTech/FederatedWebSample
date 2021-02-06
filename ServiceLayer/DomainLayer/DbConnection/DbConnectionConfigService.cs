using DbFacade.DataLayer.Models;
using ServiceLayer.DomainLayer.DbMethods;
using ServiceLayer.DomainLayer.Models.Data;
using ServiceProvider.Configuration;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Services;
using System.Collections.Generic;
using System.Linq;

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

    public interface IDbStoredApplicationSettingsConfig
    {
        void InitSettings(string appId, string enviroment);
        string Get(string name);
    }
    public class DbStoredApplicationSettingsConfig : IConfiguration, IDbStoredApplicationSettingsConfig
    {
        protected IServices Services { get; private set; }
        private IDictionary<string,string> Settings { get; set; }
        public void InitConfiguation(IServices services)
        {
            Services = services;
            ApplicationSettingsDbConnection.RegisterConnection(Services.GetConnectionString("ConsumingApplicationDb"));
        }
        public void InitSettings(string appId, string enviroment)
        {
            IEnumerable<ApplicationSetting> settings = ApplicationSettingsDbMethods.GetApplicationSettings.Execute(new DbParamsModel<string, string>(appId, enviroment));
            Settings = settings.ToDictionary(m => m.Name, m => m.Value);
        }
        public string Get(string name) => Settings.ContainsKey(name) ? Settings[name] : null;
    }
}
