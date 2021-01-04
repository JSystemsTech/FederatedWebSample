using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HtmlExtensions.FormControl;
using HtmlExtensions;
using System.Web.Mvc.Html;

namespace HtmlExtensions.Bootstrap4FormControl
{
    public interface IBootstrapFormControl : IHtmlString{
        IHtmlString AddLabel(string labelText = null);
        IHtmlString AddLabel(object htmlAttributes, string labelText = null);
        IHtmlString AddCustomLabel(string labelText = null);
        IHtmlString AddCustomLabel(object htmlAttributes, string labelText = null);

        IHtmlString AddValidation(string validationMessage = null);
        IHtmlString AddLabelAndValidation(string labelText = null, string validationMessage = null);
        IHtmlString AddLabelAndValidation(object htmlAttributes, string labelText = null, string validationMessage = null);
        IHtmlString AddCustomLabelAndValidation(string labelText = null, string validationMessage = null);
        IHtmlString AddCustomLabelAndValidation(object htmlAttributes, string labelText = null, string validationMessage = null);
    }
    internal class BootstrapFormControl<TModel, TProperty> : IBootstrapFormControl
    {
        private IHtmlString FormControl { get; set; }
        private HtmlHelper<TModel> HtmlHelper { get; set; }
        private Expression<Func<TModel, TProperty>> Expression { get; set; }

        private bool IsInvalid { get => !HtmlHelper.IsValidField(Expression); }
        private bool AppendLabel { get; set; }
        public string ToHtmlString() => FormControl.ToHtmlString();
        public BootstrapFormControl(
            IHtmlString formControl,
            HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, bool appendLabel = false)
        {
            FormControl = formControl;
            HtmlHelper = htmlHelper;
            Expression = expression;
            AppendLabel = appendLabel;
        }
        
        public IHtmlString AddLabel(string labelText = null)
        {
            return AddLabel(new { }, labelText);
        }
        public virtual IHtmlString AddLabel(object htmlAttributes, string labelText = null)
        {
            return AppendLabel ? FormControl.Append(GetLabel(htmlAttributes, labelText)) : FormControl.Prepend(GetLabel(htmlAttributes,labelText));
        }
        public IHtmlString AddCustomLabel(string labelText = null)
        {
            return AddCustomLabel(new { }, labelText);
        }
        public virtual IHtmlString AddCustomLabel(object htmlAttributes, string labelText = null)
        {
            return AppendLabel ? FormControl.Append(GetLabel(htmlAttributes, labelText, "custom-control-label")) : FormControl.Prepend(GetLabel(htmlAttributes, labelText, "custom-control-label"));
        }
        public virtual IHtmlString AddValidation(string validationMessage = null)
        {
            return  FormControl.Append(GetValidation(validationMessage));
        }
        public virtual IHtmlString AddLabelAndValidation(string labelText = null, string validationMessage = null)
        {
            return AddLabelAndValidation(new { }, labelText, validationMessage);
        }
        public virtual IHtmlString AddLabelAndValidation(object htmlAttributes, string labelText = null, string validationMessage = null)
        {
            return TemplateExtensions.Concat(GetLabel(htmlAttributes, labelText), FormControl, GetValidation(validationMessage));
        }
        public virtual IHtmlString AddCustomLabelAndValidation(string labelText = null, string validationMessage = null)
        {
            return AddLabelAndValidation(new { }, labelText, validationMessage);
        }
        public virtual IHtmlString AddCustomLabelAndValidation(object htmlAttributes, string labelText = null, string validationMessage = null)
        {
            return TemplateExtensions.Concat(GetLabel(htmlAttributes, labelText, "custom-control-label"), FormControl, GetValidation(validationMessage));
        }
        protected virtual IHtmlString GetLabel(object htmlAttributes, string labelText = null, string labelClass = "form-control-label")
        {
            IDictionary<string, object> attrs = htmlAttributes.ToHtmlAttributes();
            attrs.AddHtmlClass(labelClass);
            if (IsInvalid)
            {
                attrs.AddHtmlClass("text-danger");
            }
            HtmlHelper.ResolveRequiredClass(Expression,attrs);
            return HtmlHelper.LabelFor(Expression, labelText, attrs);
        }
        protected virtual IHtmlString GetValidation(string validationMessage = null) => HtmlHelper.ValidationMessageFor(Expression, validationMessage, new { @class= "invalid-feedback" },"div");
    }
    public static partial class Bootstrap4FormControlExtensions
    {
        private static IBootstrapFormControl ToBootstrapFormControl<TModel, TProperty>(
            this IHtmlString formControl,
            HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression, bool appendLabel = false) => new BootstrapFormControl<TModel, TProperty>(formControl, htmlHelper, expression, appendLabel);
        private static void AddBootstrapClasses(this IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes.AddHtmlClass("form-control");
        }
        private static IHtmlString AddCustomControlWrapper(this IHtmlString component, object htmlAttributes)
        {
            IDictionary<string, object> attributes = htmlAttributes.ToHtmlAttributes();
            attributes.AddHtmlClass("custom-control");
            return component.Wrap(attributes);
        }
        private static void AddBootstrapCustomControlClasses(this IDictionary<string, object> htmlAttributes, string customControlClasses = null)
        {
            htmlAttributes.AddHtmlClass(string.IsNullOrWhiteSpace(customControlClasses) ? "custom-control": customControlClasses);
        }
        private static IDictionary<string, object> ResolveBootstrapFormControlAttributes<TModel, TProperty>(
              this HtmlHelper<TModel> htmlHelper,
              Expression<Func<TModel, TProperty>> expression,
              IDictionary<string, object> htmlAttributes,
              bool customControl = false,
              string customControlClasses = null)
        {
            htmlAttributes = htmlAttributes.ToHtmlAttributes();
            if (customControl) {
                htmlAttributes.AddBootstrapCustomControlClasses(customControlClasses);
            } else { 
                htmlAttributes.AddBootstrapClasses();
            }
            
            if (!htmlHelper.IsValidField(expression))
            {
                htmlAttributes.AddHtmlClass("is-invalid");
            }
            return htmlAttributes;
        }

    }
}