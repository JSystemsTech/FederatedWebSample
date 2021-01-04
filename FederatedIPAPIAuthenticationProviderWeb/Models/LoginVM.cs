using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using System;
using System.Collections.Generic;

namespace FederatedIPAPIAuthenticationProviderWeb.Models
{
    public class LoginVM
    {
        public Guid? TestUserGuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ISiteMeta ConsumingApplicationSiteMeta { get; set; }
        public IEnumerable<ConsumerUser> TestUsers { get; set; }
        public string Mode { get; set; }
        public string OnAuthenticationMessage { get; set; }
    }
}