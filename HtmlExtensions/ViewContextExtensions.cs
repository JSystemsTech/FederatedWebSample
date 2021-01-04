using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HtmlExtensions
{
    public static class ViewContextExtensions
    {
        private static T GetValueOrDefault<T>(this RouteValueDictionary values, string key, T defaultValue = default)
        {
            object value;
            if (values.TryGetValue(key, out value))
            {
                return value is T castValue ? castValue : defaultValue;
            }
            return defaultValue;
        }
        private static string GetViewContextValue(this RouteValueDictionary values, string key) => values.GetValueOrDefault<string>(key, null);
        private static string GetRouteDataValue<TModel>(this HtmlHelper<TModel> htmlHelper, string key) => htmlHelper.ViewContext.RouteData.Values.GetViewContextValue(key);
        private static string GetRouteDataDataToken<TModel>(this HtmlHelper<TModel> htmlHelper, string key) => htmlHelper.ViewContext.RouteData.DataTokens.GetViewContextValue(key);

        public static string GetViewContextAction<TModel>(this HtmlHelper<TModel> htmlHelper) => htmlHelper.GetRouteDataValue("action");
        public static string GetViewContextController<TModel>(this HtmlHelper<TModel> htmlHelper) => htmlHelper.GetRouteDataValue("controller");
        public static string GetViewContextArea<TModel>(this HtmlHelper<TModel> htmlHelper) => htmlHelper.GetRouteDataDataToken("area");

        private static string GetRelativeViewScriptsDirectoryPath<TModel>(this HtmlHelper<TModel> htmlHelper) => htmlHelper.GetViewContextArea() is string area && !string.IsNullOrWhiteSpace(area) ? $"~/Areas/{area}/ViewScripts/": "~/Scripts/ViewScripts/";
        
        private static string GetRelativeViewScriptPath<TModel>(this HtmlHelper<TModel> htmlHelper, string file = null)
        => $"{htmlHelper.GetRelativeViewScriptsDirectoryPath()}{htmlHelper.GetViewContextController()}/{(!string.IsNullOrWhiteSpace(file) ? file : htmlHelper.GetViewContextAction())}.js";
        private static IHtmlString Empty = new MvcHtmlString(string.Empty);
        public static IHtmlString RenderViewScript<TModel>(this HtmlHelper<TModel> htmlHelper, string file = null)
        {
            string relativePath = htmlHelper.GetRelativeViewScriptPath(file);
            string absolutePath = htmlHelper.ViewContext.HttpContext.Server.MapPath(relativePath);
            return File.Exists(absolutePath) ? Scripts.Render(relativePath) : Empty;
        }

    }
}
