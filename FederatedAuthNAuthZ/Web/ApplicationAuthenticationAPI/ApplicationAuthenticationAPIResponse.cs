using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedAuthNAuthZ.Web.ConsumerAPI
{
    public class ApplicationAuthenticationApiAuthenticationResponse
    {
        public string AuthenticationToken { get; set; }
        public DateTime? AuthenticationTokenExpiration { get; set; }
        public string Message { get; set; }
    }
}
