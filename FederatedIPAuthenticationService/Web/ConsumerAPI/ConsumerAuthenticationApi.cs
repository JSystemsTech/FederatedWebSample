using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Models;

namespace FederatedIPAuthenticationService.Web.ConsumerAPI
{
    internal sealed class ConsumerApiClient: ApiClient
    {
        private ITokenProvider TokenProvider { get; set; }
        private void SetTokenAuthenticationHeader()
        {
            TokenAuthenticationHeader = new AuthenticationHeaderValue("Bearer", TokenProvider.CreateToken(
                            claims => {
                                claims.Add("Provider", new string[] { "Provider" });
                            }));
        }
        public ConsumerApiClient(ITokenProvider tokenProvider, string apiUrl) : base(apiUrl)
        {
            TokenProvider = tokenProvider;
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

    public interface IConsumerAuthenticationApi
    {
        IConsumerApplicationSettingsResponse GetConsumerApplicationSettings();
        ConsumerApiAuthenticationResponse Authenticate<T>(T data);
    }
    public sealed class ConsumerAuthenticationApi : IConsumerAuthenticationApi
    {

        private ConsumerApiClient Client { get; set; }
        private IApiEndpoint ConsumerApplicationSettingsEndpoint { get; set; }
        private IApiEndpoint AuthenticateEndpoint { get; set; } 

        public ConsumerAuthenticationApi(IEncryptionService encryptionService, ITokenProvider tokenProvider, string url)
        {
            Client = new ConsumerApiClient(tokenProvider, url);
            ConsumerApplicationSettingsEndpoint = Client.CreateEndpoint("authenticationApi/ConsumerApplicationSettings");
            AuthenticateEndpoint = Client.CreateEndpoint("authenticationApi/Authentication");

            Client.EncryptionHandler = str => encryptionService.DateSaltEncrypt(str);
            Client.DecryptionHandler = str => encryptionService.DateSaltDecrypt(str, true);
        }
        public IConsumerApplicationSettingsResponse GetConsumerApplicationSettings()=>ConsumerApplicationSettingsEndpoint.Get(new { }).Deserialize<ConsumerApplicationSettingsResponse>();
        public ConsumerApiAuthenticationResponse Authenticate<T>(T data) => AuthenticateEndpoint.Post(data).Deserialize<ConsumerApiAuthenticationResponse>();
    }

}
