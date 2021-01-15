using FederatedIPAuthenticationService.Configuration;
using ServiceProvider.ServiceProvider;

namespace FederatedIPAuthenticationService.Services
{
    public static class ServiceFactory
    {
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
