using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HtmlExtensions.FormControl;

namespace HtmlExtensions.Bootstrap4FormControl
{
    public static partial class Bootstrap4FormControlExtensions
    {
        public static IBootstrapFormControl BootstrapTextAreaFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               int rows = 1,
               int columns = 1
               )
        {
            return htmlHelper.TextAreaFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), rows, columns).ToBootstrapFormControl(htmlHelper,expression);
        }

        public static IBootstrapFormControl BootstrapTextAreaFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes,
               int rows = 1,
               int columns = 1)
        {
            return htmlHelper.BootstrapTextAreaFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), rows, columns);
        }
        public static IBootstrapFormControl BootstrapTextAreaFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               int rows = 1,
               int columns = 1)
        {
            return htmlHelper.BootstrapTextAreaFormControlFor(expression, null, rows, columns);
        }
    }
}