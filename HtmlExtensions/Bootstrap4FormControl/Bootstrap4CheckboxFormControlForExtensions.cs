using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HtmlExtensions.FormControl;
using HtmlExtensions;

namespace HtmlExtensions.Bootstrap4FormControl
{
    public static partial class Bootstrap4FormControlExtensions
    {
        #region basic checkbox
        public static IHtmlString BootstrapCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel,bool>> expression,
               IDictionary<string, object> htmlAttributes, string labelText = null, bool inline = false)
        {
            string wrapperClasses = inline ? "form-check form-check-inline" : "form-check";
            object id;
            object labelAttrs = new { @class = "form-check-label" };
            if (htmlAttributes.TryGetValue("id", out id))
            {
                labelAttrs = new { @for = id.ToString(), @class = "form-check-label" };
            }
            return htmlHelper.CheckboxFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes, true, "form-check-input"))
                .ToBootstrapFormControl(htmlHelper, expression, true).AddLabel(labelAttrs, labelText).Wrap(new {@class = wrapperClasses });
        }
        public static IHtmlString BootstrapCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               object htmlAttributes, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapCheckboxFor(expression, htmlAttributes.ToHtmlAttributes(), labelText, inline);
        }
        public static IHtmlString BootstrapCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapCheckboxFor(expression, new { }, labelText, inline);
        }
        #endregion
        #region custom checkbox
        public static IHtmlString BootstrapCustomCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               IDictionary<string, object> htmlAttributes, string labelText = null, bool inline = false)
        {
            string wrapperClasses = inline ? "custom-control custom-checkbox custom-control-inline" : "custom-control custom-checkbox";
            object id;
            object labelAttrs = new {  };
            if (htmlAttributes.TryGetValue("id", out id))
            {
                labelAttrs = new { @for = id.ToString()};
            }
            return htmlHelper.CheckboxFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes, true, "custom-control-input"))
                .ToBootstrapFormControl(htmlHelper, expression, true).AddCustomLabel(labelAttrs,labelText).Wrap(new { @class = wrapperClasses });
        }
        public static IHtmlString BootstrapCustomCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               object htmlAttributes, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapCustomCheckboxFor(expression, htmlAttributes.ToHtmlAttributes(), labelText, inline);
        }
        public static IHtmlString BootstrapCustomCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapCustomCheckboxFor(expression, new { }, labelText, inline);
        }
        #endregion
        #region custom checkbox
        public static IHtmlString BootstrapSwitchCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               IDictionary<string, object> htmlAttributes, string labelText = null, bool inline = false)
        {
            string wrapperClasses = inline ? "custom-control custom-switch custom-control-inline" : "custom-control custom-switch";
            object id;
            object labelAttrs = new { };
            if (htmlAttributes.TryGetValue("id", out id))
            {
                labelAttrs = new { @for = id.ToString() };
            }
            return htmlHelper.CheckboxFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes, true, "custom-control-input"))
                .ToBootstrapFormControl(htmlHelper, expression, true).AddCustomLabel(labelAttrs,labelText).Wrap(new { @class = wrapperClasses });
        }
        public static IHtmlString BootstrapSwitchCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               object htmlAttributes, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapSwitchCheckboxFor(expression, htmlAttributes.ToHtmlAttributes(), labelText, inline);
        }
        public static IHtmlString BootstrapSwitchCheckboxFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapSwitchCheckboxFor(expression, new { }, labelText, inline);
        }
        #endregion

    }
}