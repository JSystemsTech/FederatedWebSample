using DbFacade.DataLayer.Models;
using ServiceLayer.DomainLayer.DbMethods;
using ServiceLayer.DomainLayer.Models.Data;
using System;
using System.Linq;

namespace ServiceLayer.DomainLayer
{
    

    public interface IAuthenticationProviderDomainFacade
    {
        AuthenticationProviderApiUser AuthenticationApiUser(string username, string password);
        Guid AddWebAuthenticationCache(string url);
        string GetWebAuthenticationCache(Guid guid);
        ICACUser GetCACLoginRequestUser(string token);
    }
    internal class AuthenticationProviderDomainFacade : DomainFacadeBase, IAuthenticationProviderDomainFacade
    {
        public AuthenticationProviderApiUser AuthenticationApiUser(string username, string password) => Run(AuthenticationProviderDbMethods.AuthenticateApiUser.Execute, new DbParamsModel<string, string>(username, password)).FirstOrDefault();
        public Guid AddWebAuthenticationCache(string url) {
            var res = Run(AuthenticationProviderDbMethods.AddWebAuthenticationCache.Execute, new DbParamsModel<string>(url));
            return Guid.Parse(res.GetOutputValue("Guid").ToString());
        }
        public string GetWebAuthenticationCache(Guid guid) => Run(AuthenticationProviderDbMethods.GetWebAuthenticationCache.Execute, new DbParamsModel<Guid>(guid)).FirstOrDefault().Url;
        public ICACUser GetCACLoginRequestUser(string token) => Run(AuthenticationProviderDbMethods.GetCACLoginRequestUser.Execute, new DbParamsModel<string>(token)).FirstOrDefault();
    }
}
