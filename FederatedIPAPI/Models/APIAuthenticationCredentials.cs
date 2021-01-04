using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FederatedIPAPI.Models
{
    internal class APIAuthenticationCredentials
    {
        public string Username {get; private set;}
        public string Password { get; private set; }

        public APIAuthenticationCredentials(string encodedUsernamePassword)
        {
            string usernamePassword = Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(encodedUsernamePassword.Trim()));
            int seperatorIndex = usernamePassword.IndexOf(':');
            Username = usernamePassword.Substring(0, seperatorIndex);
            Password = usernamePassword.Substring(seperatorIndex + 1);
        }
    }
}
