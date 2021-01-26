using FederatedIPAuthenticationService.Web.ConsumerAPI;
using ServiceLayer.DomainLayer;
using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private IDomainFacade DomainFacade => Services.Get<IDomainFacade>();

        protected override void Init() { }

        public IEnumerable<ConsumerUser> GetTestUsers() => DomainFacade.GetTestUsers().Select(m=> new ConsumerUser(m.Guid, $"{m.FirstName} {m.MiddleInitial} {m.LastName}", m.Roles));
        public ConsumerUser GetTestUser(Guid userGuid) => GetTestUsers() is IEnumerable<ConsumerUser>testUsers && testUsers.Any(u=>u.Guid == userGuid)? testUsers.FirstOrDefault(u => u.Guid == userGuid): null;
        public ConsumerUser GetUser(Guid userGuid) => GetTestUser(userGuid); /*Make call to database to get user*/
        public IEnumerable<string> GetUserRoles(Guid userGuid) => GetUser(userGuid) is ConsumerUser user ? user.Roles: new string[0]; /*Make call to database to get user*/
        public ConsumerUser ResolveUser(string email, string password) => GetTestUsers().First(); /*Make call to database to get user*/
        public ConsumerUser ResolveUser(string edipi) => GetTestUsers().First(); /*Make call to database to get user*/
    }
}