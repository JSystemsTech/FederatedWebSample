using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HtmlExtensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string input)
            => string.IsNullOrWhiteSpace(input) ? input: 
            input.First().ToString().ToUpper() + input.Substring(1).ToLower();
        
    }
}