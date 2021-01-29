using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FederatedAuthNAuthZ.Web.ConsumerAPI
{
    public class ApplicationUser
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public ApplicationUser() { }
        public ApplicationUser(string userId, string name, IEnumerable<string> roles) { UserId = userId; Name = name; Roles = roles; }
    }

    public static class ApplicationAuthenticationApiExtensions
    {
        public static ApplicationUser ToConsumerUser<T>(this T model, Func<T, string> userId, Func<T, string> name, Func<T, IEnumerable<string>> roles)
        => new ApplicationUser(userId(model), name(model), roles(model));
        public static IEnumerable<ApplicationUser> ToConsumerUserList<T>(this IEnumerable<T> list, Func<T, string> userId, Func<T, string> name, Func<T, IEnumerable<string>> roles)
        => list.Select(m => m.ToConsumerUser(userId, name, roles));
    }
}
