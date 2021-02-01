using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ServiceLayer.WebHelpers.SiteMap
{
    internal class SiteMapController
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public IEnumerable<SiteMapAction> MyActions { get; set; }
        public IDictionary<string, object> ToMap()
        {
            var map = new ExpandoObject() as IDictionary<string, object>;
            MyActions.GroupBy(x => x.Key).Select(y => y.First()).ToList().ForEach(a => map.Add(a.Key, a.ToMap()));
            return map;
        }
    }
}
