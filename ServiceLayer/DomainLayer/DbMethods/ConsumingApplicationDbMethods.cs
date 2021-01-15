using DbFacade.DataLayer.CommandConfig;
using ServiceLayer.DomainLayer.DbConnection;
using ServiceLayer.DomainLayer.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.DomainLayer.DbMethods
{
    internal partial class ConsumingApplicationDbMethods
    {
        public static readonly IParameterlessDbCommandMethod<TestUser> GetTestUsers
            = ConsumingApplicationDbConnection.GetTestUsers.CreateParameterlessConfig<TestUser>();
    }
}
