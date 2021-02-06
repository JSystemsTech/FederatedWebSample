using DbFacade.DataLayer.Models;
using DbFacade.DataLayer.Models.Attributes;

namespace ServiceLayer.DomainLayer.Models.Data
{
    public class ApplicationSetting : DbDataModel
    {
        [DbColumn("Name")]
        public string Name { get; private set; }
        [DbColumn("Value")]
        public string Value { get; private set; }
    }
}
