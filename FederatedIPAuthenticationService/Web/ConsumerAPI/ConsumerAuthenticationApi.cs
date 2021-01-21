﻿using FederatedIPAuthenticationService.Services;
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
        IEnumerable<ConsumerUser> GetTestUsers();
        ConsumerApiAuthenticationResponse Authenticate(string token); 
    }
    public sealed class ConsumerAuthenticationApi : IConsumerAuthenticationApi
    {

        private ConsumerApiClient Client { get; set; }
        private IApiEndpoint TestUsersEndpoint { get; set; }
        private IApiEndpoint AuthenticateEndpoint { get; set; }

        public ConsumerAuthenticationApi(ITokenProvider tokenProvider, string url)
        {
            Client = new ConsumerApiClient(tokenProvider, url);
            TestUsersEndpoint = Client.CreateEndpoint("authenticationApi/TestUsers");
            AuthenticateEndpoint = Client.CreateEndpoint("authenticationApi/Authentication");
        }
        public IEnumerable<ConsumerUser> GetTestUsers() => TestUsersEndpoint.Get(new { }).Deserialize<IEnumerable<ConsumerUser>>();
        public ConsumerApiAuthenticationResponse Authenticate(string token) => AuthenticateEndpoint.Post(token).Deserialize<ConsumerApiAuthenticationResponse>();
    }

}