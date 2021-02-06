using ServiceProvider.Services;

namespace FederatedAuthNAuthZ.Services
{
    public interface IAPIAuthenticationService
    {
        string AuthenticationApiUser(string username, string password);
    }
    public abstract class APIAuthenticationServiceBase : Service, IAPIAuthenticationService
    {
        public abstract string AuthenticationApiUser(string username, string password);
    }
}
