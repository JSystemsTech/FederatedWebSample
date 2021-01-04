using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlExtensions.FormControl;

namespace HtmlExtensions
{
    public static class TemplateExtensions
    {
        internal static IHtmlString ToHtmlString(this string templateString) => MvcHtmlString.Create(templateString);
        private static string ToString(params IHtmlString[] templates) => string.Concat(templates.Select(html => html.ToString()));
        internal static IHtmlString Concat(params IHtmlString[] templates) => ToString(templates).ToHtmlString();
        internal static IHtmlString Concat(this IEnumerable<IHtmlString> templates) => Concat(templates.ToArray());

        internal static IHtmlString Wrap(this IHtmlString template, object attributes, string tag = "div")
        => template.Wrap(attributes.ToHtmlAttributes(), tag);
        internal static IHtmlString Wrap(this IHtmlString template, IDictionary<string, object> attributes, string tag = "div")
        {
            var tagBuilder = new TagBuilder(tag);
            tagBuilder.MergeAttributes(attributes);
            tagBuilder.InnerHtml = template.ToHtmlString();
            return tagBuilder.ToString(TagRenderMode.Normal).ToHtmlString();
        }
        internal static IHtmlString Prepend(this IHtmlString baseTemplate, IHtmlString template)
        =>(template.ToString() + baseTemplate.ToString()).ToHtmlString();
        
        internal static IHtmlString Prepend(this IHtmlString baseTemplate, params IHtmlString[] templates)
        =>(ToString(templates) + baseTemplate.ToString()).ToHtmlString();
        
        internal static IHtmlString Append(this IHtmlString baseTemplate, IHtmlString template)
        =>(baseTemplate.ToString()+ template.ToString()).ToHtmlString();
        
        internal static IHtmlString Append(this IHtmlString baseTemplate, params IHtmlString[] templates)
        =>(baseTemplate.ToString() + ToString(templates)).ToHtmlString();
        
    }
}