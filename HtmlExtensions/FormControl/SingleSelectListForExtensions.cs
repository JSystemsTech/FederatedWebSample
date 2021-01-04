using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace HtmlExtensions.FormControl
{
    public static partial class FormControlExtensions
    {
        public static IHtmlString SingleSelectFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IEnumerable<SelectListItem> selectList,
               IDictionary<string, object> htmlAttributes)
        {
            htmlHelper.ResolveRequiredHtmlAttribute(expression, htmlAttributes);
            return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        }
        public static IHtmlString SingleSelectFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IEnumerable<SelectListItem> selectList,
               object htmlAttributes)
        {
            return htmlHelper.SingleSelectFormControlFor(expression, selectList, htmlAttributes.ToHtmlAttributes());
        }
        public static IHtmlString SingleSelectFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IEnumerable<SelectListItem> selectList)
        {
            return htmlHelper.SingleSelectFormControlFor(expression, selectList, null);
        }
    }
}