using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Services;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Services;

namespace FederatedAuthNAuthZ.Services
{
    public static class ServiceFactory
    {
        public static void AddEncryptionService<TEncryptionService>(this IServiceCollection services)
            where TEncryptionService : EncryptionServiceBase, IService
        {
            services.AddConfiguration<IEncryptionServiceSettings, EncryptionServiceSettings>();
            services.AddService<TEncryptionService>();
        }

        private static void AddTokenProvider<TTokenProvider>(this IServiceCollection services)
            where TTokenProvider: class, ITokenProvider, IService
        {
            services.AddService<ITokenProvider, TTokenProvider>();
        }
        public static void AddMailService(this IServiceCollection services)
        {
            services.AddService<IMailService, MailService>();
        }
        public static void AddStandaloneTokenProvider<TTokenProviderSettings, TEncryptionService, TTokenProvider>(this IServiceCollection services)
            where TTokenProvider : class, ITokenProvider, IService
            where TEncryptionService : FederatedEncryptionService
            where TTokenProviderSettings : TokenProviderSettings
        {
            services.AddConfiguration<ITokenProviderSettings, TTokenProviderSettings>();
            services.AddService<IEncryptionService, TEncryptionService>();
            services.AddTokenProvider<TTokenProvider>();
        }
        public static void ConfigureFederatedApplication<TFederatedApplicationSettingsConfig, TEncryptionService>(this IServiceCollection services)
            where TFederatedApplicationSettingsConfig : FederatedApplicationSettingsConfig
            where TEncryptionService : FederatedEncryptionService
        {
            services.ConfigureFederatedApplication<TFederatedApplicationSettingsConfig, TEncryptionService, TokenProvider>();
        }
        public static void ConfigureFederatedApplication<TFederatedApplicationSettingsConfig, TEncryptionService, TTokenProvider>(this IServiceCollection services)
            where TTokenProvider : class, ITokenProvider, IService
            where TFederatedApplicationSettingsConfig : FederatedApplicationSettingsConfig
            where TEncryptionService : FederatedEncryptionService
        {
            services.AddConfiguration<IFederatedApplicationSettings, TFederatedApplicationSettingsConfig>();
            services.AddService<IEncryptionService, TEncryptionService>();
            services.AddTokenProvider<TTokenProvider>();
        }
        public static void AddAPIAuthenticationService<TAPIAuthenticationService>(this IServiceCollection services)
            where TAPIAuthenticationService : APIAuthenticationServiceBase
        {
            services.AddService<IAPIAuthenticationService, TAPIAuthenticationService>();
        }
        public static void AddTokenHandlerService<TTokenHandlerService>(this IServiceCollection services)
            where TTokenHandlerService : TokenHandlerServiceBase
        {
            services.AddService<ITokenHandlerService, TTokenHandlerService>();
        }
        public static void AddFederatedApplicationIdentityService<TFederatedApplicationIdentityService>(this IServiceCollection services)
            where TFederatedApplicationIdentityService : FederatedApplicationIdentityService
        {
            services.AddService<IFederatedApplicationIdentityService, TFederatedApplicationIdentityService>();
        }
        public static void AddProviderAuthenticationModeService<TProviderAuthenticationModeService>(this IServiceCollection services)
            where TProviderAuthenticationModeService : ProviderAuthenticationModeService
        {
            services.AddService<IProviderAuthenticationModeService, TProviderAuthenticationModeService>();
        }
    }
}
