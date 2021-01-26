using DbFacade.DataLayer.Models;
using DbFacade.DataLayer.Models.Attributes;
using System;

namespace ServiceLayer.DomainLayer.Models.Data
{
    public class AuthenticationProviderApiUser: DbDataModel
    {
        [DbColumn("guid")]
        public Guid Guid { get; set; }
        [DbColumn("name")]
        public string Name { get; set; }
    }
}
