using DbFacade.DataLayer.CommandConfig;
using DbFacade.DataLayer.Models;
using ServiceLayer.DomainLayer.DbConnection;
using ServiceLayer.DomainLayer.Models.Data;

namespace ServiceLayer.DomainLayer.DbMethods
{
    internal partial class ConsumingApplicationDbMethods
    {
        public static readonly IParameterlessDbCommandMethod<TestUser> GetTestUsers
            = ConsumingApplicationDbConnection.GetTestUsers.CreateParameterlessConfig<TestUser>();
    }
    internal class ApplicationSettingsDbMethods
    {
        public static readonly IDbCommandMethod<DbParamsModel<string,string>,ApplicationSetting> GetApplicationSettings
            = ApplicationSettingsDbConnection.GetApplicationSettings.CreateMethod<DbParamsModel<string, string>,ApplicationSetting>(p=> {
                p.Add("ApplicationId", p.Factory.Create(m => m.Param1));
                p.Add("Environment", p.Factory.Create(m => m.Param2));
            },
                v=> {
                    v.Add(v.Rules.Required(m => m.Param1));
                    v.Add(v.Rules.Required(m => m.Param2));
                    v.Add(v.Rules.MaxLength(m => m.Param1, 200));
                    v.Add(v.Rules.MaxLength(m => m.Param2, 50));
                });
    }
}
