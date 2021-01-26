using DbFacade.DataLayer.CommandConfig;
using DbFacade.DataLayer.Models;
using ServiceLayer.DomainLayer.DbConnection;
using ServiceLayer.DomainLayer.Models.Data;
using System;

namespace ServiceLayer.DomainLayer.DbMethods
{    
    internal class AuthenticationProviderDbMethods
    {
        public static readonly IDbCommandMethod<DbParamsModel<string, string>, AuthenticationProviderApiUser> AuthenticateApiUser
            = AuthenticationProviderDbConnection.AuthenticateApiUser.CreateMethod<DbParamsModel<string, string>, AuthenticationProviderApiUser>(p=> {
                p.Add("Username", p.Factory.Create(m => m.Param1));
                p.Add("Password", p.Factory.Create(m => m.Param2));
            },
                v=> {
                    v.Add(v.Rules.Required(m => m.Param1));
                    v.Add(v.Rules.Required(m => m.Param2));
                });
        public static readonly IDbCommandMethod<DbParamsModel<string>> AddWebAuthenticationCache
            = AuthenticationProviderDbConnection.AddWebAuthenticationCache.CreateMethod<DbParamsModel<string>>(p => {
                p.Add("Url", p.Factory.Create(m => m.Param1));
                p.Add("Guid", p.Factory.OutputGuid(100));
            },
                v => {
                    v.Add(v.Rules.Required(m => m.Param1));
                    v.Add(v.Rules.Delegate(m => m.Param1, url => {
                        bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        return result;
                    }));
                });
        public static readonly IDbCommandMethod<DbParamsModel<Guid>, WebAuthenticationCacheRecord> GetWebAuthenticationCache
            = AuthenticationProviderDbConnection.GetWebAuthenticationCache.CreateMethod<DbParamsModel<Guid>, WebAuthenticationCacheRecord>(p => {
                p.Add("Guid", p.Factory.Create(m => m.Param1));
            });
        public static readonly IDbCommandMethod<DbParamsModel<string>, CACUser> GetCACLoginRequestUser
            = AuthenticationProviderDbConnection.GetCACLoginRequestUser.CreateMethod<DbParamsModel<string>, CACUser>(p => {
                p.Add("token", p.Factory.Create(m => m.Param1));
            },v => {
                    v.Add(v.Rules.Required(m => m.Param1));
                });
    }
}
