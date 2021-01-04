using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceLayer.Services
{
    public interface IUserManagmentService {
        IEnumerable<ConsumerUser> GetTestUsers();
        ConsumerUser GetTestUser(Guid userGuid);
        ConsumerUser GetUser(Guid userGuid);
        ConsumerUser ResolveUser(string email, string password);
        ConsumerUser ResolveUser(string edipi);
        IEnumerable<string> GetUserRoles(Guid userGuid);
    }
    internal class UserManagmentService: Service, IUserManagmentService
    {
        private static ConsumerUser Admin = new ConsumerUser(new Guid("c5f68433-3c72-4628-8418-ddacd0d171a3"), "Admin T User", new string[] { "Admin", "User", "Reviewer" });
        private static ConsumerUser Reviewer = new ConsumerUser(new Guid("d732a775-3cd8-40b1-bd31-dc4b084213cd"), "Reviewer G User", new string[] { "User", "Reviewer" });
        private static ConsumerUser Normal = new ConsumerUser(new Guid("d08c8a96-909f-47b3-994e-ac57908e1213"), "Normal S User", new string[] { "User" });
        private static IEnumerable<ConsumerUser> TestUsers => new ConsumerUser[] { Admin, Reviewer, Normal };
        protected override void Init()
        {
            //ITokenProviderSettings config = Services.Get<ITokenProviderSettings>();
        }
        public IEnumerable<ConsumerUser> GetTestUsers() => TestUsers;
        public ConsumerUser GetTestUser(Guid userGuid) => TestUsers.Any(u=>u.Guid == userGuid)? TestUsers.FirstOrDefault(u => u.Guid == userGuid): null;
        public ConsumerUser GetUser(Guid userGuid) => GetTestUser(userGuid); /*Make call to database to get user*/
        public IEnumerable<string> GetUserRoles(Guid userGuid) => GetUser(userGuid) is ConsumerUser user ? user.Roles: new string[0]; /*Make call to database to get user*/
        public ConsumerUser ResolveUser(string email, string password) => Normal; /*Make call to database to get user*/
        public ConsumerUser ResolveUser(string edipi) => Normal; /*Make call to database to get user*/
    }
}