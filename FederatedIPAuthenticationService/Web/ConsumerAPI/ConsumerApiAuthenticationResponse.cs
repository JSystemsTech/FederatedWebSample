using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Web.ConsumerAPI
{
    public class ConsumerApiAuthenticationResponse
    {
        public string AuthenticationToken { get; set; }
        public DateTime? AuthenticationTokenExpiration { get; set; }
        public string Message { get; set; }
    }
}
