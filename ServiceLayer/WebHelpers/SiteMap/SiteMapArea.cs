using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ServiceLayer.WebHelpers.SiteMap
{
    internal enum SiteMapAreaType
    {
        Default,
        Area
    }
    internal class SiteMapArea
    {
        public SiteMapAreaType SiteMapAreaType { get; set; }
        public string Name { get; set; }

        public IEnumerable<string> Namespace { get; set; }

        public IEnumerable<SiteMapController> SiteMapControllers { get; set; }
        public IDictionary<string, object> ToMap()
        {
            var map = new ExpandoObject() as IDictionary<string, object>;
            SiteMapControllers.ToList().ForEach(c => map.Add(c.Name, c.ToMap()));
            return map;
        }
    }
}
