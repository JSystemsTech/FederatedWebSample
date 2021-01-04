using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FederatedIPAuthenticationService.Web.ConsumerAPI
{
    public class ConsumerUser
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public ConsumerUser() { }
        public ConsumerUser(Guid guid, string name, IEnumerable<string> roles) { Guid = guid; Name = name; Roles = roles; }
    }

    public static class ConsumenrApiExtensions
    {
        public static ConsumerUser ToConsumerUser<T>(this T model, Func<T, Guid> guid, Func<T, string> name, Func<T, IEnumerable<string>> roles)
        => new ConsumerUser(guid(model), name(model), roles(model));
        public static IEnumerable<ConsumerUser> ToConsumerUserList<T>(this IEnumerable<T> list, Func<T, Guid> guid, Func<T, string> name, Func<T, IEnumerable<string>> roles)
        => list.Select(m => m.ToConsumerUser(guid, name, roles));
    }
}
