using System;
using System.Collections.Generic;
using System.Linq;
using FederatedIPAPI.Configuration;
using Microsoft.Extensions.Options;

namespace FederatedIPAPI.Services
{
    public interface IUserService
    {
        ApiUser GetUser(string username, string password);
        ApiUser GetUser(Guid guid);
    }
    internal class UserService: IUserService
    {
        private IAPISettings APISettings { get; set; }
        public UserService(IOptions<APISettings> apiSettings)
        {
            APISettings = apiSettings.Value;
        }
        public IEnumerable<ApiUser> GetUsers()
        {
            /* TODO 
             * Do not hardcode API users like this 
             *  Create a database for user info and pull info from there
             *  This is only for demonstration purposes
             */
            List<ApiUser> users = new List<ApiUser>();

            Guid defaultUserGuid = new Guid("f0f186cb-ad6b-4df0-9e8e-cc6c009029ee");
            users.Add(ApiUser.Create(APISettings.ApiUser, APISettings.ApiPassword, defaultUserGuid, "Default Demo Client"));

            return users;
        }
        public ApiUser GetUser(string username, string password) => GetUsers().FirstOrDefault(u => u.HasValidCredentials(username, password));
        public ApiUser GetUser(Guid guid) => GetUsers().FirstOrDefault(u => u.Guid == guid);
    }
}
