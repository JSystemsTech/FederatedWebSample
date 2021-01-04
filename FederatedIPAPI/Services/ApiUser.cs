using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FederatedIPAPI.Services
{
    public class ApiUser
    {
        private string Username { get; set; }
        private string Password { get; set; }
        public Guid Guid { get; private set; }
        public string Name { get; private set; }

        public bool HasValidCredentials(string username, string password) => username == Username && password == Password;
        public static ApiUser Create(string username, string password, Guid guid, string name) => new ApiUser() { Username = username, Password = password, Guid = guid, Name = name };
    }
}
