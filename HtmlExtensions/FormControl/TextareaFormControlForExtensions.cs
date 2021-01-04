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
        public static IHtmlString TextAreaFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               int rows = 1, 
               int columns = 1
               )
        {
            htmlHelper.ResolveRequiredHtmlAttribute(expression, htmlAttributes);
            htmlHelper.ResolveMinLengthHtmlAttribute(expression, htmlAttributes);
            htmlHelper.ResolveMaxLengthHtmlAttribute(expression, htmlAttributes);
            return htmlHelper.TextAreaFor(expression,rows, columns, htmlAttributes);
        }
        
        public static IHtmlString TextAreaFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes,
               int rows = 1,
               int columns = 1)
        {
            return htmlHelper.TextAreaFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), rows, columns);
        }
        public static IHtmlString TextAreaFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               int rows = 1,
               int columns = 1)
        {
            return htmlHelper.TextAreaFormControlFor(expression, null, rows, columns);
        }

    }
}