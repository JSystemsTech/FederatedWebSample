using FederatedIPAuthenticationService.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FederatedIPAuthenticationService.Configuration
{
    public interface IConfigurationSectionConfig
    {
        IDictionary<string, string> Collection { get; }
    }
    public abstract class ConfigurationSectionConfig : IConfigurationSectionConfig, IConfiguration
    {
        public IDictionary<string, string> Collection { get; protected set; }
        protected IServices Services { get; private set; }
        protected virtual IDictionary<string, string> GetConfigurationSource()
        {
            NameValueCollection collection = ConfigurationManager.GetSection(ConfiguationSection) as NameValueCollection;
            return collection.AllKeys.ToDictionary(key => key, key => collection[key]);
        }
        public void InitConfiguation(IServices services)
        {
            Services = services;
            Collection = GetConfigurationSource();
            Init();
        }
        protected virtual void Init() { }
        protected virtual string ConfiguationSection => null;

        public ConfigurationSectionConfig() { }

        protected string GetValue(string key, bool required = false)
        {
            string value = Collection.TryGetValue(key, out string collectionValue) ? collectionValue : null;
            if (required && string.IsNullOrWhiteSpace(value))
            {
                throw new Exception($"Configuration value '{key}' for {GetType().Name} expected a value but got '{value}'");
            }
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }
    }
}
