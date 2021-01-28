using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FederatedAuthNAuthZ.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString AntiForgeryForm(this HtmlHelper helper, string id = "__AntiForgeryForm")
            => new MvcHtmlString($"<form id='{id}' class='d-none'>{helper.AntiForgeryToken()}</form>");
        public static MvcHtmlString PageLoadLink(this HtmlHelper helper, string id = "__PageLoadLink")
            => new MvcHtmlString($"<a id='{id}' href='#' class='d-none' aria-hidden='true'></a>");
    }
}