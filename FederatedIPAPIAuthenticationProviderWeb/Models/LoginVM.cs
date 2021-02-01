using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using FederatedAuthNAuthZ.Services;
using System;
using System.Collections.Generic;

namespace FederatedIPAPIAuthenticationProviderWeb.Models
{
    public class LoginVM
    {
        public IEnumerable<IAuthenticationMode> FormViewModes { get; set; }
        public string OnAuthenticationMessage { get; set; }
    }
}