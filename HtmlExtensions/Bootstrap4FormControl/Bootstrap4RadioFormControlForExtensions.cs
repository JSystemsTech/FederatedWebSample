using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HtmlExtensions.FormControl;
using System.Web.Mvc.Html;

namespace HtmlExtensions.Bootstrap4FormControl
{
    public static partial class Bootstrap4FormControlExtensions
    {
        #region basic radio
        public static IHtmlString BootstrapRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,object value, string labelText = null, bool inline = false)
        {
            string wrapperClasses = inline ? "form-check form-check-inline" : "form-check";
            object id;
            object labelAttrs = new { @class = "form-check-label" };
            if(htmlAttributes.TryGetValue("id", out id))
            {
                labelAttrs = new { @for = id.ToString(), @class = "form-check-label" };
            }
            return htmlHelper.RadioFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes, true, "form-check-input"), value)
                .ToBootstrapFormControl(htmlHelper, expression, true).AddLabel(labelAttrs, labelText).Wrap(new {@class = wrapperClasses });
        }
        public static IHtmlString BootstrapRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, object value, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapRadioFor(expression, htmlAttributes.ToHtmlAttributes(), value, labelText, inline);
        }
        public static IHtmlString BootstrapRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, object value, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapRadioFor(expression, new { }, value, labelText, inline);
        }
        #endregion
        #region custom checkbox
        public static IHtmlString BootstrapCustomRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes, object value, string labelText = null, bool inline = false)
        {
            string wrapperClasses = inline ? "custom-control custom-radio custom-control-inline" : "custom-control custom-radio";
            object id;
            object labelAttrs = new { };
            if (htmlAttributes.TryGetValue("id", out id))
            {
                labelAttrs = new { @for = id.ToString() };
            }
            return htmlHelper.RadioFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes, true, "custom-control-input"), value)
                .ToBootstrapFormControl(htmlHelper, expression, true).AddCustomLabel(labelAttrs,labelText).Wrap(new { @class = wrapperClasses });
        }
        public static IHtmlString BootstrapCustomRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, object value, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapCustomRadioFor(expression, htmlAttributes.ToHtmlAttributes(), value, labelText, inline);
        }
        public static IHtmlString BootstrapCustomRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, object value, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapCustomRadioFor(expression, new { }, value, labelText, inline);
        }
        #endregion
        #region custom checkbox
        public static IHtmlString BootstrapSwitchRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes, object value, string labelText = null, bool inline = false)
        {
            string wrapperClasses = inline ? "custom-control custom-switch custom-control-inline" : "custom-control custom-switch";
            object id;
            object labelAttrs = new { };
            if (htmlAttributes.TryGetValue("id", out id))
            {
                labelAttrs = new { @for = id.ToString() };
            }
            return htmlHelper.RadioFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes, true, "custom-control-input"), value)
                .ToBootstrapFormControl(htmlHelper, expression, true).AddCustomLabel(labelAttrs,labelText).Wrap(new { @class = wrapperClasses });
        }
        public static IHtmlString BootstrapSwitchRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, object value, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapSwitchRadioFor(expression, htmlAttributes.ToHtmlAttributes(), value, labelText, inline);
        }
        public static IHtmlString BootstrapSwitchRadioFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, object value, string labelText = null, bool inline = false)
        {
            return htmlHelper.BootstrapSwitchRadioFor(expression, new { }, value, labelText, inline);
        }
        #endregion

        #region basic radio Select
        public static IHtmlString BootstrapRadioSelectFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               SelectList options,
               IDictionary<string, object> htmlAttributes, bool inline = false)
        {
            string BaseId = htmlHelper.IdFor(m => expression).ToString();
            object id;
            object labelAttrs = new { };
            if (htmlAttributes.TryGetValue("id", out id))
            {
                BaseId= id.ToString();
            }
            List<SelectListItem> optionsList = options.ToList();
            return TemplateExtensions.Concat(optionsList.Select(item => htmlHelper.BootstrapRadioFor(expression,new { @id=$"{BaseId}{optionsList.IndexOf(item)}"}, item.Value, item.Text, inline)));
        }
        public static IHtmlString BootstrapRadioSelectFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               SelectList options,
               object htmlAttributes, bool inline = false)
        {
            return htmlHelper.BootstrapRadioSelectFor(expression, options, htmlAttributes.ToHtmlAttributes(), inline);
        }
        public static IHtmlString BootstrapRadioSelectFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, SelectList options, bool inline = false)
        {
            return htmlHelper.BootstrapRadioSelectFor(expression, options, new { }, inline);
        }
        #endregion
        private static object GetRadioSelectAttrs(this SelectListItem item, string id)
        {
            if (item.Selected) {
                return new { @checked = true, @id = id };
            }
            return new { @id = id };
        }
        #region custom radio Select
        public static IHtmlString BootstrapCustomRadioSelectFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               SelectList options,
               IDictionary<string, object> htmlAttributes, bool inline = false)
        {
            string BaseId = htmlHelper.IdFor(m => expression).ToString();
            object id;
            object labelAttrs = new { };
            if (htmlAttributes.TryGetValue("id", out id))
            {
                BaseId = id.ToString();
            }
            List<SelectListItem> optionsList = options.ToList();
            return TemplateExtensions.Concat(optionsList.Select(item => htmlHelper.BootstrapCustomRadioFor(expression, item.GetRadioSelectAttrs($"{BaseId}{optionsList.IndexOf(item)}"), item.Value, item.Text, inline)));
        }
        public static IHtmlString BootstrapCustomRadioSelectFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               SelectList options,
               object htmlAttributes, bool inline = false)
        {
            return htmlHelper.BootstrapCustomRadioSelectFor(expression, options, htmlAttributes.ToHtmlAttributes(), inline);
        }
        public static IHtmlString BootstrapCustomRadioSelectFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, SelectList options, bool inline = false)
        {
            return htmlHelper.BootstrapCustomRadioSelectFor(expression, options, new { }, inline);
        }
        #endregion



    }
}