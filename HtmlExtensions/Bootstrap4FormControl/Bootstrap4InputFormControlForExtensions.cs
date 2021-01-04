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
       #region color
        public static IBootstrapFormControl BootstrapColorInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.ColorInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression,htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapColorInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapColorInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapColorInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapColorInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region date
        public static IBootstrapFormControl BootstrapDateInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.DateInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapDateInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapDateInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapDateInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapDateInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region datetime-local
        public static IBootstrapFormControl BootstrapDateTimeLocalInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.DateTimeLocalInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapDateTimeLocalInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapDateTimeLocalInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapDateTimeLocalInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapDateTimeLocalInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region email
        public static IBootstrapFormControl BootstrapEmailInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.EmailInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapEmailInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapEmailInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapEmailInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapEmailInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region file
        public static IBootstrapFormControl BootstrapFileInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.FileInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes,true,"form-control-file"), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapFileInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapFileInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapFileInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapFileInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region month
        public static IBootstrapFormControl BootstrapMonthInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.MonthInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapMonthInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapMonthInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapMonthInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapMonthInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region number
        public static IBootstrapFormControl BootstrapNumberInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.NumberInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapNumberInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapNumberInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapNumberInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapNumberInputFormControlFor(expression, new { }, format);
        }
        #endregion        
        #region password
        public static IBootstrapFormControl BootstrapPasswordInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.PasswordInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapPasswordInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapPasswordInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapPasswordInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapPasswordInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region tel
        public static IBootstrapFormControl BootstrapTelInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.TelInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapTelInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapTelInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapTelInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapTelInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region text
        public static IBootstrapFormControl BootstrapTextInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.TextInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapTextInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapTextInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapTextInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapTextInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region time
        public static IBootstrapFormControl BootstrapTimeInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.TimeInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapTimeInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapTimeInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapTimeInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapTimeInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region url
        public static IBootstrapFormControl BootstrapUrlInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.UrlInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapUrlInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapUrlInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapUrlInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapUrlInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region week
        public static IBootstrapFormControl BootstrapWeekInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.WeekInputFormControlFor(expression, htmlHelper.ResolveBootstrapFormControlAttributes(expression, htmlAttributes), format).ToBootstrapFormControl(htmlHelper,expression);
        }
        public static IBootstrapFormControl BootstrapWeekInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.BootstrapWeekInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IBootstrapFormControl BootstrapWeekInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.BootstrapWeekInputFormControlFor(expression, new { }, format);
        }
        #endregion
    }
}