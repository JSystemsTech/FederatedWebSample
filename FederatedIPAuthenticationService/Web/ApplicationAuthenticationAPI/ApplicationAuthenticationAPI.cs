using FederatedAuthNAuthZ.Services;
using FederatedIPAuthenticationService.Extensions;
using ServiceProviderShared;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClient;

namespace FederatedAuthNAuthZ.Web.ConsumerAPI
{
    internal sealed class ApplicationAuthenticationApiApiClient : ApiClient
    {
        private ITokenProvider TokenProvider => ServiceManager.GetService<ITokenProvider>();
        private void SetTokenAuthenticationHeader()
        {
            TokenAuthenticationHeader = new AuthenticationHeaderValue("Bearer", TokenProvider.CreateToken(
                            claims => {
                                claims.Add("Provider", new string[] { "Provider" });
                            }));
        }
        public ApplicationAuthenticationApiApiClient(string apiUrl) : base(apiUrl)
        {
            SetTokenAuthenticationHeader();
        }
        protected override void Authenticate() {
            SetTokenAuthenticationHeader();
        }
        protected override async Task AuthenticateAsync()
        {
            SetTokenAuthenticationHeader();
            await Task.CompletedTask;
        }
    }

    public interface IApplicationAuthenticationAPI
    {
        IApplicationAuthenticationAPIApplicationSettingsResponse GetApplicationSettings();
        ApplicationAuthenticationApiAuthenticationResponse Authenticate<T>(T data);
    }
    public sealed class ApplicationAuthenticationAPI : IApplicationAuthenticationAPI
    {

        private ApplicationAuthenticationApiApiClient Client { get; set; }
        private IApiEndpoint ApplicationSettingsEndpoint { get; set; }
        private IApiEndpoint AuthenticateEndpoint { get; set; }
        private IEncryptionService EncryptionService => ServiceManager.GetService<IEncryptionService>();
        public ApplicationAuthenticationAPI(string url)
        {
            Client = new ApplicationAuthenticationApiApiClient(url);
            ApplicationSettingsEndpoint = Client.CreateEndpoint("ApplicationSettings".GetApplicationAuthenticationAPIEndpointName());
            AuthenticateEndpoint = Client.CreateEndpoint("Authentication".GetApplicationAuthenticationAPIEndpointName());

            Client.EncryptionHandler = str => EncryptionService.DateSaltEncrypt(str);
            Client.DecryptionHandler = str => EncryptionService.DateSaltDecrypt(str, true);
        }
        public IApplicationAuthenticationAPIApplicationSettingsResponse GetApplicationSettings()=> ApplicationSettingsEndpoint.Get(new { }).Deserialize<ApplicationAuthenticationAPIApplicationSettingsResponse>();
        public ApplicationAuthenticationApiAuthenticationResponse Authenticate<T>(T data) => AuthenticateEndpoint.Post(data).Deserialize<ApplicationAuthenticationApiAuthenticationResponse>();
    }

}
