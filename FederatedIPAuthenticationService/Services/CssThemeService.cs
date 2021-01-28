using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace FederatedAuthNAuthZ.Services
{
    public interface ICssThemeService
    {
        IDictionary<string, ThemeBundle> Themes { get; }
        ThemeBundle GetTheme(string key);
    }
    public class ThemeBundle : StyleBundle
    {
        public string Theme { get; private set; }
        public bool IsDarkTheme { get; private set; }

        private static TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;
        private static string PathPrefix = "~/Content/css/theme/";

        public ThemeBundle(bool isDarkTheme = false) : this("Default", isDarkTheme) { }
        public ThemeBundle(string theme, bool isDarkTheme = false) : base($"{PathPrefix}{theme}") {
            Theme = TextInfo.ToTitleCase(theme);
            IsDarkTheme = isDarkTheme;
        }
        public ThemeBundle(string theme, string cdnPath, bool isDarkTheme = false) : base($"{PathPrefix}{theme}", cdnPath)
        {
            Theme = TextInfo.ToTitleCase(theme);
            IsDarkTheme = isDarkTheme;
        }
    }
    public  class CssThemeService: Service, ICssThemeService
    {
        public IDictionary<string, ThemeBundle> Themes { get; set; }

        protected override void Init()
        {
            base.Init();
            Themes = BundleTable.Bundles.Where(b => b is ThemeBundle).Select(b => (ThemeBundle)b).ToDictionary(t => t.Theme, t => t);
        }
        public ThemeBundle GetTheme(string key) => !string.IsNullOrWhiteSpace(key) && Themes.ContainsKey(key) ? Themes[key] : Themes["Default"];
        
    }
}