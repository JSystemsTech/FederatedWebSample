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
        public ConsumerApiClient(string apiUrl) : base(apiUrl)
        {
            InitialAuthenticationHeader = new AuthenticationHeaderValue("Bearer", "");
            TokenAuthenticationHeader = InitialAuthenticationHeader;
        }
        public void RefreshAuthenticationToken(string token) {
            TokenAuthenticationHeader = new AuthenticationHeaderValue("Bearer", token);            
        }
        protected override void Authenticate() { }
        protected override async Task AuthenticateAsync()
        {
            await Task.CompletedTask;
        }
    }

    public interface IConsumerApi {
        IEnumerable<ConsumerUser> GetTestUsers(string token);
        ConsumerApiAuthenticateResponse Authenticate(string token); 
    }
    public sealed class ConsumerApi : IConsumerApi {

        private ConsumerApiClient Client { get; set; }
        private IApiEndpoint TestUsersEndpoint { get; set; }
        private IApiEndpoint AuthenticateEndpoint { get; set; }

        public ConsumerApi(string url)
        {
            Client = new ConsumerApiClient(url);
            TestUsersEndpoint = Client.CreateEndpoint("GetTestUsers");
            AuthenticateEndpoint = Client.CreateEndpoint("Authenticate");
        }
        private TResponse GetValue<TData, TResponse>(IApiEndpoint endpoint, string token, TData data)
        {
            Client.RefreshAuthenticationToken(token);
            IApiResponse response = endpoint.Get(data);
            return response.Deserialize<TResponse>();
        }
        private TResponse GetValue<TResponse>(IApiEndpoint endpoint, string token) => GetValue<object, TResponse>(endpoint, token, new { });
        public IEnumerable<ConsumerUser> GetTestUsers(string token) => GetValue<IEnumerable<ConsumerUser>>(TestUsersEndpoint, token);
        public ConsumerApiAuthenticateResponse Authenticate(string token) => GetValue<ConsumerApiAuthenticateResponse>(AuthenticateEndpoint, token);
    }

}
