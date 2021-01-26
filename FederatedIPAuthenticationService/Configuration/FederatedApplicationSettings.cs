using FederatedIPAuthenticationService.Extensions;
using ServiceProvider.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FederatedIPAuthenticationService.Configuration
{
    internal class FederatedApplicationSettingsFields {
        public static string SiteId => "SiteId";
        public static string SiteRhealmId => "SiteRhealmId";
        public static string SiteReturnUrl => "SiteReturnUrl";
        public static string SiteName => "SiteName";
        public static string SiteVersion => "SiteVersion";
        public static string SiteEnvironment => "SiteEnvironment";
        public static string SiteNetwork => "SiteNetwork";
        public static string SiteNetworkDisplay => "SiteNetworkDisplay";
        public static string SiteNetworkDescription => "SiteNetworkDescription";
        public static string SiteDescription => "SiteDescription";
        public static string ConsumerAuthenticationApiUrl => "ConsumerAuthenticationApiUrl";
        public static string Theme => "Theme";
        public static string DarkTheme => "DarkTheme";
        public static string RequirePrivacyPolicy => "RequirePrivacyPolicy";
        public static string PrivacyPolicyUrl => "PrivacyPolicyUrl";
        public static string CookiePolicyUrl => "CookiePolicyUrl";
        public static string AuthenticationModes => "AuthenticationModes";


        public static string UseSessionCookie => "UseSessionCookie";
        public static string LogoutUrl => "LogoutUrl";
        public static string AuthenticationProviderId => "AuthenticationProviderId";
        public static string AuthenticationProviderUrl => "AuthenticationProviderUrl";
        public static string TokenProviderAuthenticationEndpoint => "TokenProviderAuthenticationEndpoint";
        public static string TokenProviderUsername => "TokenProviderUsername";
        public static string TokenProviderPassword => "TokenProviderPassword";

        public static string IsProvider => "IsProvider";
        public static string ExternalAuthenticationtUrl => "ExternalAuthenticationtUrl";
        public static string AuthenticationErrorUrl => "AuthenticationErrorUrl";
    }

    public static class FederatedApplicationSettingsExtensions
    {
        private static IEnumerable<string> CommonProductionEnvironmentNames = new string[] { "prod", "production" };
        public static bool IsProductionEnvironment(this IFederatedApplicationSettings FederatedApplicationSettings) => CommonProductionEnvironmentNames.Contains(FederatedApplicationSettings.SiteEnvironment.ToLower().Trim());
        public static string VersionDisplay(this IFederatedApplicationSettings FederatedApplicationSettings) => FederatedApplicationSettings.IsProductionEnvironment() ? $"-v {FederatedApplicationSettings.SiteVersion}": $"-v {FederatedApplicationSettings.SiteVersion} - {FederatedApplicationSettings.SiteEnvironment}";
        public static string Display(this IFederatedApplicationSettings FederatedApplicationSettings) => $"{FederatedApplicationSettings.SiteName} {FederatedApplicationSettings.VersionDisplay()}";
        public static string GetEnvironment(this IFederatedApplicationSettings FederatedApplicationSettings) => FederatedApplicationSettings.IsProductionEnvironment() ? "" : FederatedApplicationSettings.SiteEnvironment;
        public static bool UseRhealm(this IFederatedApplicationSettings FederatedApplicationSettings) => !string.IsNullOrWhiteSpace(FederatedApplicationSettings.SiteRhealmId);
        public static string GetCookieSuffix(this IFederatedApplicationSettings FederatedApplicationSettings) => FederatedApplicationSettings.UseRhealm() ? FederatedApplicationSettings.SiteRhealmId : $"{FederatedApplicationSettings.SiteId}{FederatedApplicationSettings.GetEnvironment()}{FederatedApplicationSettings.SiteVersion}";
        public static bool IsSameNetwork(this IFederatedApplicationSettings FederatedApplicationSettings, IFederatedApplicationSettings FederatedApplicationSettingsTarget) => FederatedApplicationSettings.SiteNetwork == FederatedApplicationSettingsTarget.SiteNetwork;
        public static void UpdateConsumerTokenClaims(this IFederatedApplicationSettings FederatedApplicationSettings, IDictionary<string, IEnumerable<string>> claims)
        {
            claims.AddUpdate(FederatedApplicationSettingsFields.SiteNetwork, FederatedApplicationSettings.SiteNetwork);
            claims.AddUpdate(FederatedApplicationSettingsFields.SiteRhealmId, FederatedApplicationSettings.SiteRhealmId);
            claims.AddUpdate(FederatedApplicationSettingsFields.SiteId, FederatedApplicationSettings.SiteId);
            claims.AddUpdate(FederatedApplicationSettingsFields.SiteVersion, FederatedApplicationSettings.SiteVersion);
            claims.AddUpdate(FederatedApplicationSettingsFields.SiteEnvironment, FederatedApplicationSettings.SiteEnvironment);
        }
        public static bool ValidateConsumerTokenClaims(this IFederatedApplicationSettings FederatedApplicationSettings, IDictionary<string, IEnumerable<string>> claims)
        {
            return FederatedApplicationSettings.UseRhealm() ? FederatedApplicationSettings.ValidateConsumerTokenRhealmIdentificationClaims(claims) || FederatedApplicationSettings.ValidateConsumerTokenSiteIdentificationClaims(claims) :
                FederatedApplicationSettings.ValidateConsumerTokenSiteIdentificationClaims(claims);
        }
        public static bool ValidateConsumerTokenSiteIdentificationClaims(this IFederatedApplicationSettings FederatedApplicationSettings, IDictionary<string, IEnumerable<string>> claims)
        {
            return claims.ContainsKey(FederatedApplicationSettingsFields.SiteNetwork) && claims[FederatedApplicationSettingsFields.SiteNetwork].FirstOrDefault() == FederatedApplicationSettings.SiteNetwork &&
            (claims.ContainsKey(FederatedApplicationSettingsFields.SiteId) && claims[FederatedApplicationSettingsFields.SiteId].FirstOrDefault() == FederatedApplicationSettings.SiteId) &&
            (claims.ContainsKey(FederatedApplicationSettingsFields.SiteVersion) && claims[FederatedApplicationSettingsFields.SiteVersion].FirstOrDefault() == FederatedApplicationSettings.SiteVersion) &&
            (claims.ContainsKey(FederatedApplicationSettingsFields.SiteEnvironment) && claims[FederatedApplicationSettingsFields.SiteEnvironment].FirstOrDefault() == FederatedApplicationSettings.SiteEnvironment);
        }
        public static bool ValidateConsumerTokenRhealmIdentificationClaims(this IFederatedApplicationSettings FederatedApplicationSettings, IDictionary<string, IEnumerable<string>> claims)
        {
            return claims.ContainsKey(FederatedApplicationSettingsFields.SiteNetwork) && claims[FederatedApplicationSettingsFields.SiteNetwork].FirstOrDefault() == FederatedApplicationSettings.SiteNetwork &&
            claims.ContainsKey(FederatedApplicationSettingsFields.SiteRhealmId) && claims[FederatedApplicationSettingsFields.SiteRhealmId].FirstOrDefault() == FederatedApplicationSettings.SiteRhealmId;
        }
        public static string GetNetworkDisplay(this IFederatedApplicationSettings FederatedApplicationSettings) => string.IsNullOrWhiteSpace(FederatedApplicationSettings.SiteNetworkDisplay) ? FederatedApplicationSettings.SiteNetwork : FederatedApplicationSettings.SiteNetworkDisplay;
        public static bool HasNetworkDescription(this IFederatedApplicationSettings FederatedApplicationSettings) => !string.IsNullOrWhiteSpace(FederatedApplicationSettings.SiteNetworkDescription);
        public static bool NetworkIs(this IFederatedApplicationSettings FederatedApplicationSettings, string network) => FederatedApplicationSettings.SiteNetwork == network;

    }
    public interface IFederatedApplicationSettings
    {
        string SiteId { get; }
        string SiteRhealmId { get; }
        
        string SiteReturnUrl { get; }
        string SiteName { get; }
        string SiteVersion { get; }
        string SiteEnvironment { get; }
        string SiteNetwork { get; }
        string SiteNetworkDisplay { get; }
        string SiteNetworkDescription { get; }
        string SiteDescription { get; }        
        string ConsumerAuthenticationApiUrl { get; }
        string Theme { get; }
        string DarkTheme { get; }
        bool RequirePrivacyPolicy { get;}
        string PrivacyPolicyUrl { get; }
        string CookiePolicyUrl { get; }
        IEnumerable<string> AuthenticationModes { get; }


        bool UseSessionCookie { get; }
        string LogoutUrl { get; }
        string AuthenticationProviderId { get; }
        string AuthenticationProviderUrl { get; }
        string TokenProviderAuthenticationEndpoint { get; }
        string TokenProviderUsername { get; }
        string TokenProviderPassword { get; }


        bool IsProvider { get; }
        string ExternalAuthenticationtUrl { get; }
        string AuthenticationErrorUrl { get; }

        IDictionary<string, string> Collection { get; }
        
    }

    public class FederatedApplicationSettings : IFederatedApplicationSettings {
        public string SiteId { get; set; }
        public string SiteRhealmId { get; set; }
        public string SiteReturnUrl { get; set; }
        public string SiteName { get; set; }
        public string SiteVersion { get; set; }
        public string SiteEnvironment { get; set; }
        public string SiteNetwork { get; set; }
        public string SiteNetworkDisplay { get; set; }
        public string SiteNetworkDescription { get; set; }
        public string SiteDescription { get; set; }
        public string ConsumerAuthenticationApiUrl { get; set; }
        public string Theme { get; set; }
        public string DarkTheme { get; set; }
        public bool RequirePrivacyPolicy { get; set; }
        public string PrivacyPolicyUrl { get; set; }
        public string CookiePolicyUrl { get; set; }
        public IEnumerable<string> AuthenticationModes { get; set; }
        private static IDictionary<string, string> EmptyCollection = new Dictionary<string, string>();

        public bool UseSessionCookie { get; set; }
        public string LogoutUrl { get; set; }
        public string AuthenticationProviderId { get; set; }
        public string AuthenticationProviderUrl { get; set; }
        public string TokenProviderAuthenticationEndpoint { get; set; }
        public string TokenProviderUsername { get; set; }
        public string TokenProviderPassword { get; set; }


        public bool IsProvider { get; set; }
        public string ExternalAuthenticationtUrl { get; set; }
        public string AuthenticationErrorUrl { get; set; }

        public IDictionary<string, string> Collection => EmptyCollection;
        public FederatedApplicationSettings(){ }
        public FederatedApplicationSettings(IFederatedApplicationSettings FederatedApplicationSettings) {
            SiteId = FederatedApplicationSettings.SiteId;
            SiteRhealmId = FederatedApplicationSettings.SiteRhealmId;
            SiteReturnUrl = FederatedApplicationSettings.SiteReturnUrl;
            SiteName = FederatedApplicationSettings.SiteName;
            SiteVersion = FederatedApplicationSettings.SiteVersion;
            SiteEnvironment = FederatedApplicationSettings.SiteEnvironment;
            SiteNetwork = FederatedApplicationSettings.SiteNetwork;
            SiteNetworkDisplay = FederatedApplicationSettings.SiteNetworkDisplay;
            SiteNetworkDescription = FederatedApplicationSettings.SiteNetworkDescription;
            SiteDescription = FederatedApplicationSettings.SiteDescription;
            ConsumerAuthenticationApiUrl = FederatedApplicationSettings.ConsumerAuthenticationApiUrl;
            Theme = FederatedApplicationSettings.Theme;
            DarkTheme = FederatedApplicationSettings.DarkTheme;
            RequirePrivacyPolicy = FederatedApplicationSettings.RequirePrivacyPolicy;
            PrivacyPolicyUrl = FederatedApplicationSettings.PrivacyPolicyUrl;
            CookiePolicyUrl = FederatedApplicationSettings.CookiePolicyUrl;
            AuthenticationModes = FederatedApplicationSettings.AuthenticationModes;

            UseSessionCookie = FederatedApplicationSettings.UseSessionCookie;
            LogoutUrl = FederatedApplicationSettings.LogoutUrl;
            AuthenticationProviderId = FederatedApplicationSettings.AuthenticationProviderId;
            AuthenticationProviderUrl = FederatedApplicationSettings.AuthenticationProviderUrl;
            //TokenProviderAuthenticationEndpoint = FederatedApplicationSettings.TokenProviderAuthenticationEndpoint;
            //TokenProviderUsername = FederatedApplicationSettings.TokenProviderUsername;
            //TokenProviderPassword = FederatedApplicationSettings.TokenProviderPassword;

            IsProvider = FederatedApplicationSettings.IsProvider;
            ExternalAuthenticationtUrl = FederatedApplicationSettings.ExternalAuthenticationtUrl;
            AuthenticationErrorUrl = FederatedApplicationSettings.AuthenticationErrorUrl;
        }
    }

    internal class FederatedApplicationSettingsConfig : ConfigurationSectionConfig, IFederatedApplicationSettings, IConfigurationSectionConfig
    {
        private static string FederatedApplicationSettings = "federatedApplicationSettings";
        protected override string ConfiguationSection => FederatedApplicationSettings;
        public string SiteId { get; set; }
        public string SiteRhealmId { get; set; }
        public string SiteReturnUrl { get; set; }
        public string SiteName { get; set; }
        public string SiteVersion { get; set; }
        public string SiteEnvironment { get; set; }
        public string SiteNetwork { get; set; }
        public string SiteNetworkDisplay { get; set; }
        public string SiteNetworkDescription { get; set; }
        public string SiteDescription { get; set; }
        public string ConsumerAuthenticationApiUrl { get; set; }
        public string Theme { get; set; }
        public string DarkTheme { get; set; }
        public bool RequirePrivacyPolicy { get; set; }
        public string PrivacyPolicyUrl { get; set; }
        public string CookiePolicyUrl { get; set; }
        public IEnumerable<string> AuthenticationModes { get; set; }


        public bool UseSessionCookie { get; set; }
        public string LogoutUrl { get; set; }
        public string AuthenticationProviderId { get; set; }
        public string AuthenticationProviderUrl { get; set; }
        public string TokenProviderAuthenticationEndpoint { get; set; }
        public string TokenProviderUsername { get; set; }
        public string TokenProviderPassword { get; set; }

        public bool IsProvider { get; set; }
        public string ExternalAuthenticationtUrl { get; set; }
        public string AuthenticationErrorUrl { get; set; }

        public FederatedApplicationSettingsConfig() : base() { }
        public FederatedApplicationSettingsConfig(IDictionary<string, string> collection) : base() { Collection = collection; Init(); }
        
        protected override void Init()
        {
            SiteId = GetValue(FederatedApplicationSettingsFields.SiteId, true);                        
            SiteName = GetValue(FederatedApplicationSettingsFields.SiteName, true);
            SiteVersion = GetValue(FederatedApplicationSettingsFields.SiteVersion, true);
            SiteEnvironment = GetValue(FederatedApplicationSettingsFields.SiteEnvironment, true);
            SiteNetwork = GetValue(FederatedApplicationSettingsFields.SiteNetwork, true);
            SiteDescription = GetValue(FederatedApplicationSettingsFields.SiteDescription, true);

            if (bool.TryParse(GetValue(FederatedApplicationSettingsFields.IsProvider), out bool isProvider))
            {
                IsProvider = isProvider;
            }

            if (IsProvider) {
                ExternalAuthenticationtUrl = GetValue(FederatedApplicationSettingsFields.ExternalAuthenticationtUrl, true);
                AuthenticationErrorUrl = GetValue(FederatedApplicationSettingsFields.AuthenticationErrorUrl, true);
            } 
            else
            {
                SiteRhealmId = GetValue(FederatedApplicationSettingsFields.SiteRhealmId);
                SiteNetworkDisplay = GetValue(FederatedApplicationSettingsFields.SiteNetworkDisplay);
                SiteNetworkDescription = GetValue(FederatedApplicationSettingsFields.SiteNetworkDescription);
                SiteReturnUrl = GetValue(FederatedApplicationSettingsFields.SiteReturnUrl, true);
                ConsumerAuthenticationApiUrl = GetValue(FederatedApplicationSettingsFields.ConsumerAuthenticationApiUrl, true);
                Theme = GetValue(FederatedApplicationSettingsFields.Theme);
                DarkTheme = GetValue(FederatedApplicationSettingsFields.DarkTheme);
                if (bool.TryParse(GetValue(FederatedApplicationSettingsFields.RequirePrivacyPolicy), out bool requirePrivacyPolicy))
                {
                    RequirePrivacyPolicy = requirePrivacyPolicy;
                }
                PrivacyPolicyUrl = GetValue(FederatedApplicationSettingsFields.PrivacyPolicyUrl, RequirePrivacyPolicy);
                CookiePolicyUrl = GetValue(FederatedApplicationSettingsFields.CookiePolicyUrl, true);
                AuthenticationModes = GetValue(FederatedApplicationSettingsFields.AuthenticationModes, true).Split(',');

                if (bool.TryParse(GetValue(FederatedApplicationSettingsFields.UseSessionCookie), out bool useSessionCookie))
                {
                    UseSessionCookie = useSessionCookie;
                }
                LogoutUrl = GetValue(FederatedApplicationSettingsFields.LogoutUrl, true);
                AuthenticationProviderId = GetValue(FederatedApplicationSettingsFields.AuthenticationProviderId, true);
                AuthenticationProviderUrl = GetValue(FederatedApplicationSettingsFields.AuthenticationProviderUrl, true);
                TokenProviderAuthenticationEndpoint = GetValue(FederatedApplicationSettingsFields.TokenProviderAuthenticationEndpoint,true);
                TokenProviderUsername = GetValue(FederatedApplicationSettingsFields.TokenProviderUsername,true);
                TokenProviderPassword = GetValue(FederatedApplicationSettingsFields.TokenProviderPassword,true);
            }
            
        }
    }
}
