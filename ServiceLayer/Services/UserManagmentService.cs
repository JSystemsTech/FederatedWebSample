using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using ServiceLayer.DomainLayer;
using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayer.Services
{
    public interface IUserManagmentService {
        IEnumerable<ApplicationUser> GetTestUsers();
        ApplicationUser GetTestUser(string userId);
        ApplicationUser GetUser(string userId);
        ApplicationUser ResolveUser(string email, string password);
        ApplicationUser ResolveUser(string edipi);
        IEnumerable<string> GetUserRoles(string userId);
    }
    internal class UserManagmentService: Service, IUserManagmentService
    {

        private IDomainFacade DomainFacade => Services.Get<IDomainFacade>();
        //private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        protected override void Init() { }

        public IEnumerable<ApplicationUser> GetTestUsers() => DomainFacade.GetTestUsers().Select(m=> new ApplicationUser(m.Guid.ToString(), $"{m.FirstName} {m.MiddleInitial} {m.LastName}", m.Roles));
        //private Guid? DecryptUserGuid(string userId) => EncryptionService.DateSaltDecrypt(userId, true) is string userGuidStr && Guid.TryParse(userGuidStr, out Guid guid) ? guid : default;
        public ApplicationUser GetTestUser(string userId) => GetTestUsers() is IEnumerable<ApplicationUser> testUsers && testUsers.Any(u=> u.UserId == userId) ? testUsers.FirstOrDefault(u => u.UserId == userId) : null;
        public ApplicationUser GetUser(string userId) => GetTestUser(userId); /*Make call to database to get user*/
        public IEnumerable<string> GetUserRoles(string userId) => GetUser(userId) is ApplicationUser user ? user.Roles: new string[0]; /*Make call to database to get user*/
        public ApplicationUser ResolveUser(string email, string password) => GetTestUsers().First(); /*Make call to database to get user*/
        public ApplicationUser ResolveUser(string edipi) => GetTestUsers().First(); /*Make call to database to get user*/
    }
}