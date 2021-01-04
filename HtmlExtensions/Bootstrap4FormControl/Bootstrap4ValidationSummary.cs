using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace HtmlExtensions.Bootstrap4FormControl
{
    public static partial class Bootstrap4FormControlExtensions
    {
        private static IHtmlString BuildSummaryErrorMessage<TModel>(this HtmlHelper<TModel> htmlHelper,ModelError error) 
            => $"<p class=\"bs-validation-error mb-0\">{htmlHelper.Encode(error.ErrorMessage)}</p>".ToHtmlString();
        private static IHtmlString BuildSummaryErrorMessagesForKeyErrors<TModel>(this HtmlHelper<TModel> htmlHelper, ModelErrorCollection errors)
            => errors.Select(e=> htmlHelper.BuildSummaryErrorMessage(e)).Concat();
        private static IHtmlString BuildSummaryListItem<TModel>(this HtmlHelper<TModel> htmlHelper, string key)
            => htmlHelper.ViewData.ModelState[key].Errors.Any() ? 
            $"<li class=\"bs-validation-item\"><p class=\"font-weight-bold mb-1\">{key}</p>{htmlHelper.BuildSummaryErrorMessagesForKeyErrors(htmlHelper.ViewData.ModelState[key].Errors)}</li>".ToHtmlString()
            : string.Empty.ToHtmlString();
        private static IHtmlString BuildSummaryListItems<TModel>(this HtmlHelper<TModel> htmlHelper)
            => htmlHelper.ViewData.ModelState.Keys.Select(k => htmlHelper.BuildSummaryListItem(k)).Concat();
        private static IHtmlString BuildSummaryList<TModel>(this HtmlHelper<TModel> htmlHelper)
            => $"<ul class=\"bs-validation-list\">{htmlHelper.BuildSummaryListItems()}</ul>".ToHtmlString();

        public static IHtmlString BootstrapValidatonSummary<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               string type = "danger")
        {        
 
            if (!htmlHelper.ViewData.ModelState.IsValid)
            {
                return $"<div class=\"my-1 alert alert-{type}\" role=\"alert\">{htmlHelper.BuildSummaryList()}</div>".ToHtmlString();
            }
            return string.Empty.ToHtmlString();
        }
    }
}