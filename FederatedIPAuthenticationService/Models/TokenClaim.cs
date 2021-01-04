using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FederatedIPAuthenticationService.Models
{
    internal class TokenClaim
    {
        public string Name { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}