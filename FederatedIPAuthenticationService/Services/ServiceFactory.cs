using FederatedAuthNAuthZ.Configuration;
using FederatedIPAuthenticationService.Services;
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
        private static void AddTokenProvider(this IServiceCollection services)
        {
            services.AddTokenProvider<TokenProvider>();
        }
        public static void AddMailService(this IServiceCollection services)
        {
            services.AddService<IMailService, MailService>();
        }
        public static void ConfigureIndependantFederatedTokenProvider(this IServiceCollection services)
        {
            services.AddConfiguration<ITokenProviderSettings, TokenProviderSettings>();
            services.AddTokenProvider();
        }
        public static void ConfigureFederatedApplication(this IServiceCollection services)
        {
            services.ConfigureFederatedApplication<TokenProvider>();
        }
        public static void ConfigureFederatedApplication<TTokenProvider>(this IServiceCollection services)
            where TTokenProvider : class, ITokenProvider, IService
        {
            services.AddConfiguration<IFederatedApplicationSettings, FederatedApplicationSettingsConfig>();
            services.AddService<IEncryptionService, FederatedApplicationBasicEncryptionService>();
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

    }
}
