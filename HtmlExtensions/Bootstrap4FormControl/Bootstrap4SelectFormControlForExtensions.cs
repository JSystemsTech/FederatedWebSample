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
        public static IBootstrapFormControl BootstrapSingleSelectFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IEnumerable<SelectListItem> selectList,
               IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.SingleSelectFormControlFor(expression, selectList, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes)).ToBootstrapFormControl(htmlHelper, expression);
        }
        public static IBootstrapFormControl BootstrapSingleSelectFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IEnumerable<SelectListItem> selectList,
               object htmlAttributes)
        {
            return htmlHelper.BootstrapSingleSelectFormControlFor(expression, selectList, htmlAttributes.ToHtmlAttributes());
        }
        public static IBootstrapFormControl BootstrapSingleSelectFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IEnumerable<SelectListItem> selectList)
        {
            return htmlHelper.BootstrapSingleSelectFormControlFor(expression, selectList, null);
        }

        public static IBootstrapFormControl BootstrapMultiSelectFormControlFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.MultiSelectFormControlFor(expression, selectList, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes)).ToBootstrapFormControl(htmlHelper, expression);
        }
        public static IBootstrapFormControl BootstrapMultiSelectFormControlFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return htmlHelper.BootstrapMultiSelectFormControlFor(expression, selectList, htmlAttributes.ToHtmlAttributes());
        }
        public static IBootstrapFormControl BootstrapMultiSelectFormControlFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList)
        {
            return htmlHelper.BootstrapMultiSelectFormControlFor(expression, selectList, null);
        }
    }
}