using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace FederatedIPAuthenticationService.ServiceProvider
{
    public interface IServiceCollection
    {
        void AddSingleton<TIService, TService>(TService instance)
                where TService : class, TIService;
        void AddSingleton<TService>(TService instance) where TService : class;
        void AddSingleton<TService>(Func<IServices, TService> builder) where TService : class;

        void AddService<TIService, TService>()
            where TService : IService, TIService;
        void AddService<TService>() where TService : IService;
        void AddConfiguration<TIService, TService>()
            where TService : IConfiguration, TIService;
        void AddConfiguration<TService>() where TService : IConfiguration;
    }
        public interface IServices
    {
        T Get<T>();
    }
    public interface IService {
        void InitService(IServices services);
    }
    public interface IConfiguration
    {
        void InitConfiguation(IServices services);
    }
    public interface ISpecialConfiguration: IConfiguration { }
    internal class ServiceCollection : IServiceCollection, IServices
    {
        private Dictionary<Type, object> Services { get; set; }
        public ServiceCollection(Action<IServiceCollection> configureServices)
        {
            Services = new Dictionary<Type, object>();
            configureServices(this);
            InitSpecialConfigurations();
            InitConfigurations();
            InitServices();
        }
        public void AddSingleton<TService>(Func<IServices, TService> builder) where TService : class => AddSingleton(builder(this));
        public void AddSingleton<TIService, TService>(TService instance)
            where TService : class, TIService
        {
            if (!Services.ContainsKey(typeof(TIService)))
            {
                Services.Add(typeof(TIService), instance);
            }
        }
        public void AddSingleton<TService>(TService instance) where TService : class => AddSingleton<TService, TService>(instance);

        public void AddService<TIService, TService>()
            where TService : IService, TIService
        {
            if (!Services.ContainsKey(typeof(TIService)))
            {
                Services.Add(typeof(TIService), (TService)Activator.CreateInstance(typeof(TService)));
            }
        }
        public void AddService<TService>() where TService : IService => AddService<TService, TService>();

        public void AddConfiguration<TIService, TService>()
            where TService : IConfiguration, TIService
        {
            if (!Services.ContainsKey(typeof(TIService)))
            {
                Services.Add(typeof(TIService), (TService)Activator.CreateInstance(typeof(TService)));
            }
        }      

        public void AddConfiguration<TService>() where TService : IConfiguration => AddConfiguration<TService, TService>();

        private void InitSpecialConfigurations() => Services.Where(i => i.Value is ISpecialConfiguration).ToList().ForEach(i => {
            IConfiguration config = (IConfiguration)i.Value;
            config.InitConfiguation(this);
        });
        private void InitConfigurations() => Services.Where(i => i.Value is IConfiguration && !(i.Value is ISpecialConfiguration)).ToList().ForEach(i => {
            IConfiguration config = (IConfiguration)i.Value;
            config.InitConfiguation(this);
        });
        private void InitServices() => Services.Where(i => i.Value is IService).ToList().ForEach(i => {
            IService config = (IService)i.Value;
            config.InitService(this);
        });
        
        public T Get<T>() => Services.ContainsKey(typeof(T)) ? (T)Services[typeof(T)] : default;
    }
}
