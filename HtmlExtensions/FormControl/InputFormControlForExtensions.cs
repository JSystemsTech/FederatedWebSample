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
        private const string InputTypeButton = "button";
        private const string InputTypeCheckbox = "checkbox";
        private const string InputTypeColor = "color";
        private const string InputTypeDate = "date";
        private const string InputTypeDateTimeLocal = "datetime-local";
        private const string InputTypeEmail = "email";
        private const string InputTypeFile = "file";
        private const string InputTypeHidden = "hidden";
        private const string InputTypeImage = "image";
        private const string InputTypeMonth = "month";
        private const string InputTypeNumber = "number";
        private const string InputTypePassword = "password";
        private const string InputTypeRadio = "radio";
        private const string InputTypeRange = "range";
        private const string InputTypeReset = "reset";
        private const string InputTypeTel = "tel";
        private const string InputTypeText = "text";
        private const string InputTypeTime = "time";
        private const string InputTypeUrl = "url";
        private const string InputTypeWeek = "week";

        

        public static IHtmlString InputFormControlForCore<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string type = InputTypeText, string format = null)
        {
            htmlAttributes.SetHtmlAttribute("type", type);
            htmlHelper.ResolveRequiredHtmlAttribute(expression, htmlAttributes);
            htmlHelper.ResolveMinLengthHtmlAttribute(expression, htmlAttributes);
            htmlHelper.ResolveMaxLengthHtmlAttribute(expression, htmlAttributes);
            htmlAttributes.SetAutoComplete();
            htmlAttributes.SetPattern(format);
            return htmlHelper.TextBoxFor(expression, htmlAttributes);
        }
        public static IHtmlString InputFormControlForCore<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes,
               string type = InputTypeText, string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes.ToHtmlAttributes(), type,format);
        }
        public static IHtmlString InputFormControlForCore<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               string type = InputTypeText, string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, null, type,format);
        }
        public delegate IHtmlString InputFormControlForWithObjectHtmlParamsDelegate();
        #region color
        public static IHtmlString ColorInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeDate, format);
        }
        public static IHtmlString ColorInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.ColorInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),format);
        }
        public static IHtmlString ColorInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.ColorInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region date
        public static IHtmlString DateInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {           
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeDate, format);
        }
        public static IHtmlString DateInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.DateInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),format);
        }
        public static IHtmlString DateInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.DateInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region datetime-local
        public static IHtmlString DateTimeLocalInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeDateTimeLocal, format);
        }
        public static IHtmlString DateTimeLocalInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.DateTimeLocalInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),format);
        }
        public static IHtmlString DateTimeLocalInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.DateTimeLocalInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region email
        public static IHtmlString EmailInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeEmail, format);
        }
        public static IHtmlString EmailInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.EmailInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),format);
        }
        public static IHtmlString EmailInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.EmailInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region file
        public static IHtmlString FileInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeFile, format);
        }
        public static IHtmlString FileInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.FileInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),format);
        }
        public static IHtmlString FileInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.FileInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region month
        public static IHtmlString MonthInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeMonth, format);
        }
        public static IHtmlString MonthInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.MonthInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),format);
        }
        public static IHtmlString MonthInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.MonthInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region number
        public static IHtmlString NumberInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            string defaultStep = typeof(TProperty) == typeof(decimal) ? "any" : null;
            htmlAttributes.SetStep(defaultStep);
            htmlHelper.ResolveMinHtmlAttribute(expression, htmlAttributes);
            htmlHelper.ResolveMaxHtmlAttribute(expression, htmlAttributes);
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeNumber, format);
        }
        public static IHtmlString NumberInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.NumberInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),format);
        }
        public static IHtmlString NumberInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.NumberInputFormControlFor(expression, new { }, format);
        }
        #endregion        
        #region password
        public static IHtmlString PasswordInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypePassword, format);
        }
        public static IHtmlString PasswordInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.PasswordInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IHtmlString PasswordInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.PasswordInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region tel
        public static IHtmlString TelInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeTel, format);
        }
        public static IHtmlString TelInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.TelInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IHtmlString TelInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.TelInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region text
        public static IHtmlString TextInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, format);
        }
        public static IHtmlString TextInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.TextInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IHtmlString TextInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.TextInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region time
        public static IHtmlString TimeInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeTime, format);
        }
        public static IHtmlString TimeInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.TimeInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IHtmlString TimeInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.TimeInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region url
        public static IHtmlString UrlInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeUrl, format);
        }
        public static IHtmlString UrlInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.UrlInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IHtmlString UrlInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.UrlInputFormControlFor(expression, new { }, format);
        }
        #endregion
        #region week
        public static IHtmlString WeekInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               string format = null)
        {
            return htmlHelper.InputFormControlForCore(expression, htmlAttributes, InputTypeWeek, format);
        }
        public static IHtmlString WeekInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes, string format = null)
        {
            return htmlHelper.WeekInputFormControlFor(expression, htmlAttributes.ToHtmlAttributes(), format);
        }
        public static IHtmlString WeekInputFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, string format = null)
        {
            return htmlHelper.WeekInputFormControlFor(expression, new { }, format);
        }
        #endregion

        #region checkbox
        public static IHtmlString CheckboxFormControlForCore<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               IDictionary<string, object> htmlAttributes)
        {
            htmlHelper.ResolveRequiredHtmlAttribute(expression, htmlAttributes);
            return htmlHelper.CheckBoxFor(expression, htmlAttributes);
        }
        public static IHtmlString CheckboxFormControlFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.CheckboxFormControlForCore(expression, htmlAttributes);
        }
        public static IHtmlString CheckboxFormControlFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression,
               object htmlAttributes)
        {
            return htmlHelper.CheckboxFormControlFor(expression, htmlAttributes.ToHtmlAttributes());
        }
        public static IHtmlString CheckboxFormControlFor<TModel>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, bool>> expression)
        {
            return htmlHelper.CheckboxFormControlFor(expression, new { });
        }
        #endregion
        #region radio
        public static IHtmlString RadioFormControlForCore<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               object value)
        {
            htmlHelper.ResolveRequiredHtmlAttribute(expression, htmlAttributes);
            return htmlHelper.RadioButtonFor(expression, value, htmlAttributes);
        }
        public static IHtmlString RadioFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               IDictionary<string, object> htmlAttributes,
               object value)
        {
            return htmlHelper.RadioFormControlForCore(expression, htmlAttributes, value);
        }
        public static IHtmlString RadioFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object htmlAttributes,
               object value)
        {
            return htmlHelper.RadioFormControlFor(expression, htmlAttributes.ToHtmlAttributes(),value);
        }
        public static IHtmlString RadioFormControlFor<TModel, TProperty>(
               this HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               object value)
        {
            return htmlHelper.RadioFormControlFor(expression, new { }, value);
        }
        #endregion
    }
}