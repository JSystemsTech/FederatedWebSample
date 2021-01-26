using DbFacade.DataLayer.Models;
using DbFacade.DataLayer.Models.Attributes;
using System;


namespace ServiceLayer.DomainLayer.Models.Data
{
    public class WebAuthenticationCacheRecord : DbDataModel
    {
        [DbColumn("Url")]
        public string Url { get; set; }
    }
}
