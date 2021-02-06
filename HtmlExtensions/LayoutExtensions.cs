using System.Web.Mvc;

namespace HtmlExtensions
{
    public static class LayoutExtensions
    {
        public static MvcHtmlString AntiForgeryForm(this HtmlHelper helper, string id = "__AntiForgeryForm")
            => new MvcHtmlString($"<form id='{id}' class='d-none'>{helper.AntiForgeryToken()}</form>");
        public static MvcHtmlString PageLoadLink(this HtmlHelper helper, string id = "__PageLoadLink")
            => new MvcHtmlString($"<a id='{id}' href='#' class='d-none' aria-hidden='true' tabindex='-1'></a>");
    }
}
