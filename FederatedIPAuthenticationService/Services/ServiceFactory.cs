using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.ServiceProvider;

namespace FederatedIPAuthenticationService.Services
{
    public static class ServiceFactory
    {
        public static void AddApplicationSettings<TIApplicationSettings, TApplicationSettings>(this IServiceCollection services)
            where TApplicationSettings : ApplicationSettings, TIApplicationSettings
        {
            services.AddConfiguration<TIApplicationSettings, TApplicationSettings>();
        }
        public static void AddApplicationSettings<TApplicationSettings>(this IServiceCollection services)
            where TApplicationSettings : ApplicationSettings
        {
            services.AddConfiguration<IApplicationSettings, TApplicationSettings>();
        }
        public static void AddApplicationSettings(this IServiceCollection services)
        {
            services.AddConfiguration<IApplicationSettings, ApplicationSettings>();
        }


        public static void AddConnectionStringConfig<TIConnectionStringConfig, TConnectionStringConfig>(this IServiceCollection services)
            where TConnectionStringConfig : ConnectionStringConfig, TIConnectionStringConfig
        {
            services.AddConfiguration<TIConnectionStringConfig, TConnectionStringConfig>();
        }
        public static void AddConnectionStringConfig<TConnectionStringConfig>(this IServiceCollection services)
            where TConnectionStringConfig : ConnectionStringConfig
        {
            services.AddConfiguration<IConnectionStringConfig, TConnectionStringConfig>();
        }
        public static void AddConnectionStringConfig(this IServiceCollection services)
        {
            services.AddConfiguration<IConnectionStringConfig, ConnectionStringConfig>();
        }

        public static void AddAuthenticationRequestCache(this IServiceCollection services) {
            services.AddService<IAuthenticationRequestCache, AuthenticationRequestCache>();
        }
        public static void AddEncryptionService(this IServiceCollection services)
        {
            services.AddConfiguration<IEncryptionServiceSettings, EncryptionServiceSettings>();
            services.AddService<IEncryptionService, EncryptionService>();
        }
        public static void AddTokenProvider(this IServiceCollection services)
        {
            services.AddConfiguration<ITokenProviderSettings, TokenProviderSettings>();
            services.AddService<ITokenProvider, TokenProvider>();
        }
        public static void AddMailService(this IServiceCollection services)
        {
            services.AddService<IMailService, MailService>();
        }
        public static void ConfigureAsFederatedProvider(this IServiceCollection services)
        {
            services.AddConfiguration<ISiteMeta, SiteMeta>();
            services.AddConfiguration<IFederatedProviderSettings, FederatedProviderSettings>();
        }
        public static void ConfigureAsFederatedConsumer(this IServiceCollection services)
        {
            services.AddConfiguration<ISiteMeta, SiteMeta>();
            services.AddConfiguration<IFederatedConsumerSettings, FederatedConsumerSettings>();
        }
    }
}
