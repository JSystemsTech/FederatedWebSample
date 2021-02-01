using System.Web.Mvc;

namespace ServiceLayer.WebHelpers.SiteMap
{
    internal class SiteMapAction
    {
        internal string Key { get => $"{Name}{AjaxMethod}"; }
        public string Name { get; set; }

        public string AjaxMethod { get; set; }
        public bool IsHttpPost { get; set; }

        public bool ReturnsJson { get; set; }
        public bool ReturnsPartialView { get; set; }
        public bool IsLink { get; set; }
        public string Url { get; private set; }
        internal void SetUrl(UrlHelper url, SiteMapArea area, SiteMapController controller)
            => Url = area.SiteMapAreaType == SiteMapAreaType.Default ? url.Action(Name, controller.Name) : url.Action(Name, controller.Name, new { Area = area.Name });
        internal object ToMap() => new { AjaxMethod, ReturnsJson, ReturnsPartialView, IsLink, Url };
    }
}
