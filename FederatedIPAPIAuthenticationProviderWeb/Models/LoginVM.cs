using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using System;
using System.Collections.Generic;

namespace FederatedIPAPIAuthenticationProviderWeb.Models
{
    public class LoginVM
    {
        public string TestUserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFederatedApplicationSettings ConsumingApplicationFederatedApplicationSettings { get; set; }
        public IEnumerable<ApplicationUser> TestUsers { get; set; }
        public string Mode { get; set; }
        public string OnAuthenticationMessage { get; set; }
    }
}