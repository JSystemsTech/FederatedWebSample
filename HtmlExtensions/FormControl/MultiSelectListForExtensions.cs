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
        public static IHtmlString MultiSelectFormControlFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            htmlHelper.ResolveRequiredHtmlAttribute(expression, htmlAttributes);
            return htmlHelper.ListBoxFor(expression, selectList, htmlAttributes);
        }
        public static IHtmlString MultiSelectFormControlFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return htmlHelper.MultiSelectFormControlFor(expression, selectList, htmlAttributes.ToHtmlAttributes());
        }
        public static IHtmlString MultiSelectFormControlFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList)
        {
            return htmlHelper.MultiSelectFormControlFor(expression, selectList, null);
        }

    }
}