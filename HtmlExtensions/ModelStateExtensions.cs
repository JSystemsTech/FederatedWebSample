using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HtmlExtensions
{
    public static class ModelStateExtensions
    {
        private static MemberExpression GetMemberExpression(this Expression expression)
        {
            if (expression is MemberExpression)
            {
                return (MemberExpression)expression;
            }
            else if (expression is LambdaExpression)
            {
                var lambdaExpression = expression as LambdaExpression;
                if (lambdaExpression.Body is MemberExpression)
                {
                    return (MemberExpression)lambdaExpression.Body;
                }
                else if (lambdaExpression.Body is UnaryExpression)
                {
                    return ((MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand);
                }
            }
            return null;
        }
        private static string GetPropertyPath(this Expression expr)
        {
            var path = new StringBuilder();
            MemberExpression memberExpression = expr.GetMemberExpression();
            do
            {
                if (path.Length > 0)
                {
                    path.Insert(0, ".");
                }
                path.Insert(0, memberExpression.Member.Name);
                memberExpression = memberExpression.Expression.GetMemberExpression();
            }
            while (memberExpression != null);
            return path.ToString();
        }
        public static void AddModelError<TModel, TParam>(this TModel model,ModelStateDictionary ModelState, Expression<Func<TModel, TParam>> property, string errorMessage)
        {
            ModelState.AddModelError(property.GetPropertyPath(), errorMessage);
        }
        public static void AddModelError<TModel, TParam>(this TModel model, ModelStateDictionary ModelState, Expression<Func<TModel, TParam>> property, Exception e)
        {
            ModelState.AddModelError(property.GetPropertyPath(), e);
        }
        public static bool IsValidField<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        => htmlHelper.ViewData.ModelState.IsValidField(property.GetPropertyPath());
        
    }
}