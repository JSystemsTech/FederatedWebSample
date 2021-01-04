using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace HtmlExtensions.FormControl
{
    public static partial class FormControlExtensions
    {
        internal static IDictionary<string, object> ToHtmlAttributes(this object htmlAttributes) 
            => htmlAttributes is IDictionary<string, object> attrs? attrs : 
            htmlAttributes == null ? System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(new { }): 
            System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        internal static object GetHtmlAttribute(this IDictionary<string, object> htmlAttributes, string key)
        {
            if (htmlAttributes.ContainsHtmlAttribute(key))
            {
                object returnVal;
                return htmlAttributes.TryGetValue(key, out returnVal) ? returnVal :
                    htmlAttributes.TryGetValue($"@{key}", out returnVal) ? returnVal :
                    htmlAttributes.TryGetValue(key.Capitalize(), out returnVal) ? returnVal :
                    htmlAttributes.TryGetValue($"@{key.Capitalize()}", out returnVal) ? returnVal : null;
            }
            return null;
        }
        internal static IEnumerable<string> GetHtmlClasses(this IDictionary<string, object> htmlAttributes)
        {
            object value = htmlAttributes.GetHtmlAttribute("class");
            return value != null ? value.ToString().Split(' ').Select(c => c.Trim()): new string[0];
        }
        internal static void AddHtmlClass(this IDictionary<string, object> htmlAttributes, string className)
        {
            IEnumerable<string> classes = htmlAttributes.GetHtmlClasses();
            if (!classes.Contains(className))
            {
                List<string> updatedClasses = classes.ToList();
                updatedClasses.Add(className.Trim());
                htmlAttributes.SetHtmlAttribute("class", string.Join(" ", updatedClasses.ToArray()));
            }
        }
        internal static void RemoveHtmlClass(this IDictionary<string, object> htmlAttributes, string className)
        {
            IEnumerable<string> classes = htmlAttributes.GetHtmlClasses();
            if (!classes.Contains(className))
            {
                classes.ToList().RemoveAll(c=> c == className.Trim());
                htmlAttributes.SetHtmlAttribute("class", string.Join(" ", classes.ToArray()));
            }
        }

        private static MemberExpression GetPropertyMemberExpression<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Not a memberExpression");

            if (!(memberExpression.Member is PropertyInfo))
                throw new InvalidOperationException("Not a property");

            return memberExpression;
        }
        internal static IEnumerable<ValidationAttribute> GetValidationAttributes<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            MemberExpression memberExpression = GetPropertyMemberExpression(htmlHelper, expression);
            return memberExpression.Member.GetCustomAttributes(true).OfType<ValidationAttribute>();
        }
        internal static bool HasValidationAttribute<TValidationAttribute>(this IEnumerable<ValidationAttribute> validationAttributes)
        where TValidationAttribute: ValidationAttribute
        {
            return validationAttributes.Any(v => v.GetType() == typeof(TValidationAttribute));
        }
        internal static TValidationAttribute GetValidationAttribute<TValidationAttribute>(this IEnumerable<ValidationAttribute> validationAttributes)
        where TValidationAttribute : ValidationAttribute
        {
            return validationAttributes.FirstOrDefault(v => v.GetType() == typeof(TValidationAttribute)) as TValidationAttribute;
        }
        internal static bool HasRequiredValidationAttribute(this IEnumerable<ValidationAttribute> validationAttributes)
            => validationAttributes.HasValidationAttribute<RequiredAttribute>();

        internal static bool ContainsHtmlAttribute(this IDictionary<string, object> htmlAttributes, string key)
        {
            return htmlAttributes.ContainsKey(key) || htmlAttributes.ContainsKey(key.Capitalize()) || htmlAttributes.ContainsKey($"@{key}") || htmlAttributes.ContainsKey($"@{key.Capitalize()}");
        }
        internal static bool HasRequiredHtmlAttribute(this IDictionary<string, object> htmlAttributes) => htmlAttributes.ContainsHtmlAttribute("required");
        internal static bool HasTypeHtmlAttribute(this IDictionary<string, object> htmlAttributes) => htmlAttributes.ContainsHtmlAttribute("type");

        internal static void RemoveHtmlAttribute(this IDictionary<string, object> htmlAttributes, string key)
        {
            htmlAttributes.Remove(key);
            htmlAttributes.Remove(key.Capitalize());
            htmlAttributes.Remove($"@{key}");
            htmlAttributes.Remove($"@{key.Capitalize()}");
        }
        internal static void SetHtmlAttribute(this IDictionary<string, object> htmlAttributes, string key, object value)
        {
            htmlAttributes.RemoveHtmlAttribute(key);
            htmlAttributes.Add(key, value);
        }
        internal static void ResolveRequiredClass<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes = null)
        {
            IEnumerable<ValidationAttribute> validationAttributes = htmlHelper.GetValidationAttributes(expression);
            if (!htmlAttributes.ContainsHtmlAttribute("required") && validationAttributes.HasRequiredValidationAttribute())
            {
                htmlAttributes.AddHtmlClass("required");
            }
        }
        internal static void ResolveRequiredHtmlAttribute<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes = null)
        {
            IEnumerable<ValidationAttribute> validationAttributes = htmlHelper.GetValidationAttributes(expression);
            if (!htmlAttributes.ContainsHtmlAttribute("required") && validationAttributes.HasRequiredValidationAttribute())
            {
                htmlAttributes.SetHtmlAttribute("required", "");
            }
        }
        internal static  void ResolveMaxLengthHtmlAttribute<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes = null)
        {
            string htmlAttributeName = "maxlength";
            if (!htmlAttributes.ContainsHtmlAttribute(htmlAttributeName))
            {
                IEnumerable<ValidationAttribute> validationAttributes = htmlHelper.GetValidationAttributes(expression);

                if (validationAttributes.HasValidationAttribute<StringLengthAttribute>())
                {
                    StringLengthAttribute attribute = validationAttributes.GetValidationAttribute<StringLengthAttribute>();
                    htmlAttributes.SetHtmlAttribute(htmlAttributeName, attribute.MaximumLength);
                }
                else if (validationAttributes.HasValidationAttribute<MaxLengthAttribute>())
                {
                    MaxLengthAttribute attribute = validationAttributes.GetValidationAttribute<MaxLengthAttribute>();
                    htmlAttributes.SetHtmlAttribute(htmlAttributeName, attribute.Length);
                }
            }
               
        }
        internal static void ResolveMinLengthHtmlAttribute<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes = null)
        {
            string htmlAttributeName = "minlength";
            if (!htmlAttributes.ContainsHtmlAttribute(htmlAttributeName))
            {
                IEnumerable<ValidationAttribute> validationAttributes = htmlHelper.GetValidationAttributes(expression);

                if (validationAttributes.HasValidationAttribute<StringLengthAttribute>())
                {
                    StringLengthAttribute attribute = validationAttributes.GetValidationAttribute<StringLengthAttribute>();
                    htmlAttributes.SetHtmlAttribute(htmlAttributeName, attribute.MinimumLength);
                }
                else if (validationAttributes.HasValidationAttribute<MinLengthAttribute>())
                {
                    MinLengthAttribute attribute = validationAttributes.GetValidationAttribute<MinLengthAttribute>();
                    htmlAttributes.SetHtmlAttribute(htmlAttributeName, attribute.Length);
                }
            }
        }
        internal static void ResolveMinHtmlAttribute<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes = null)
        {
            IEnumerable<ValidationAttribute> validationAttributes = htmlHelper.GetValidationAttributes(expression);

            if (!htmlAttributes.ContainsHtmlAttribute("min") && validationAttributes.HasValidationAttribute<RangeAttribute>())
            {
                RangeAttribute attribute = validationAttributes.GetValidationAttribute<RangeAttribute>();
                htmlAttributes.SetHtmlAttribute("min", attribute.Minimum);
            }
        }
        internal static void ResolveMaxHtmlAttribute<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IDictionary<string, object> htmlAttributes = null)
        {
            IEnumerable<ValidationAttribute> validationAttributes = htmlHelper.GetValidationAttributes(expression);

            if (!htmlAttributes.ContainsHtmlAttribute("max") && validationAttributes.HasValidationAttribute<RangeAttribute>())
            {
                RangeAttribute attribute = validationAttributes.GetValidationAttribute<RangeAttribute>();
                htmlAttributes.SetHtmlAttribute("max", attribute.Maximum);
            }
        }
        internal static void SetAutoComplete(
            this IDictionary<string, object> htmlAttributes, string value ="off")
        {
            string formattedValue = value.Trim().ToLower();
            formattedValue = formattedValue == "on" || formattedValue == "off" ? formattedValue : "off";
            if (!htmlAttributes.ContainsHtmlAttribute("autocomplete"))
            {
                htmlAttributes.SetHtmlAttribute("autocomplete", formattedValue);
            }
        }
        internal static void SetPlaceholder(
            this IDictionary<string, object> htmlAttributes, string placeholder)
        {
            if (!htmlAttributes.ContainsHtmlAttribute("placeholder"))
            {
                htmlAttributes.SetHtmlAttribute("placeholder", placeholder);
            }
        }
        internal static void SetPattern(
            this IDictionary<string, object> htmlAttributes, string pattern = null)
        {
            if (!htmlAttributes.ContainsHtmlAttribute("pattern") && !string.IsNullOrWhiteSpace(pattern))
            {
                htmlAttributes.SetHtmlAttribute("pattern", pattern);
            }
        }
        internal static void SetStep(
            this IDictionary<string, object> htmlAttributes, string step = null)
        {
            if (!htmlAttributes.ContainsHtmlAttribute("step") && !string.IsNullOrWhiteSpace(step))
            {
                htmlAttributes.SetHtmlAttribute("step", step);
            }
        }

        public static IHtmlString ApplyLabelAndValidation<TModel, TProperty>(
            this IHtmlString formControl,
            HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               Func<HtmlHelper<TModel>, Expression<Func<TModel, TProperty>>, IHtmlString> labelResolver,
            Func<HtmlHelper<TModel>, Expression<Func<TModel, TProperty>>, IHtmlString> validationResolver
            )
        {
            return TemplateExtensions.Concat(labelResolver(htmlHelper, expression), formControl, validationResolver(htmlHelper, expression));
        }
        public static IHtmlString ApplyLabel<TModel, TProperty>(
            this IHtmlString formControl,
            HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               Func<HtmlHelper<TModel>, Expression<Func<TModel, TProperty>>, IHtmlString> labelResolver
            )
        {
            return formControl.Prepend(labelResolver(htmlHelper, expression));
        }
        public static IHtmlString ApplyValidation<TModel, TProperty>(
            this IHtmlString formControl,
            HtmlHelper<TModel> htmlHelper,
               Expression<Func<TModel, TProperty>> expression,
               Func<HtmlHelper<TModel>, Expression<Func<TModel, TProperty>>, IHtmlString> validationResolver
            )
        {
            return formControl.Append(validationResolver(htmlHelper, expression));
        }
    }
}