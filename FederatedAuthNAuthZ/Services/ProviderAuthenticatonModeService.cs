using FederatedAuthNAuthZ.Web.ConsumerAPI;
using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FederatedAuthNAuthZ.Services
{
    public interface IProviderAuthenticationModeService {
        ApplicationAuthenticationApiAuthenticationResponse Authenticate(string mode, IApplicationAuthenticationAPI applicationAuthenticationAPI, object values);
        IEnumerable<IAuthenticationMode> GetAuthenticatonModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes);
        IEnumerable<IAuthenticationMode> GetFormViewModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes);
        IEnumerable<IAuthenticationMode> GetRedirectOnlyModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes);
        IAuthenticationMode GetMode(string mode);
    }
    public interface IAuthenticationMode {
        bool ImediatelyAuthenticate { get;  }
        bool IsExternalService { get; }
        string RedirectUrl { get; }
        string FormView { get;  }
        string Mode { get; }
    }
    internal class AuthenticationMode: IAuthenticationMode {
        public bool ImediatelyAuthenticate { get; set; }
        public bool IsExternalService { get; set; }
        public string RedirectUrl { get; set; }
        public string FormView { get; set; }
        public string Mode { get; set; }
    }
    public abstract class ProviderAuthenticationModeService : Service, IProviderAuthenticationModeService
    {
        private IDictionary<string, Func<IApplicationAuthenticationAPI, object, ApplicationAuthenticationApiAuthenticationResponse>> AuthenticationMethods { get; set; }
        private IDictionary<string, IAuthenticationMode> AuthenticatonModes { get; set; }


        public IEnumerable<IAuthenticationMode> GetAuthenticatonModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes)
            => AuthenticatonModes.Where(m => FilterAuthenticatonModes(appApi, applicationRequestedModes).Contains(m.Key)).Select(kv => kv.Value);
        protected virtual IEnumerable<string> FilterAuthenticatonModes(IApplicationAuthenticationAPI appApi,IEnumerable<string> applicationRequestedModes) => applicationRequestedModes;
        public IEnumerable<IAuthenticationMode> GetFormViewModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes)
            => GetAuthenticatonModes(appApi, applicationRequestedModes).Where(m=> !string.IsNullOrWhiteSpace(m.FormView));
        public IEnumerable<IAuthenticationMode> GetRedirectOnlyModes(IApplicationAuthenticationAPI appApi, IEnumerable<string> applicationRequestedModes)
            => GetAuthenticatonModes(appApi, applicationRequestedModes).Where(m => m.IsExternalService && string.IsNullOrWhiteSpace(m.FormView));
        protected sealed override void Init()
        {
            base.Init();
            AuthenticationMethods = new Dictionary<string, Func<IApplicationAuthenticationAPI, object, ApplicationAuthenticationApiAuthenticationResponse>>();
            AuthenticatonModes = new Dictionary<string, IAuthenticationMode>();
            RegisterAuthenticatonModes();
        }
        protected abstract void RegisterAuthenticatonModes();
        public ApplicationAuthenticationApiAuthenticationResponse Authenticate(string mode, IApplicationAuthenticationAPI applicationAuthenticationAPI, object values)
        => AuthenticationMethods.ContainsKey(mode) ? AuthenticationMethods[mode](applicationAuthenticationAPI, values) : null;

        protected void RegisterAuthenticationMethod(string mode, Func<IApplicationAuthenticationAPI,object, ApplicationAuthenticationApiAuthenticationResponse> validationMethod)
            => AuthenticationMethods.Add(mode, validationMethod);
        protected void RegisterFormViewMode(string mode, string formView)
            => AuthenticatonModes.Add(mode, new AuthenticationMode { Mode = mode, FormView = formView});

        protected void RegisterFormRedirectMode(string mode, string formView, string redirectUrl)
            => AuthenticatonModes.Add(mode, new AuthenticationMode { Mode = mode, IsExternalService = true, RedirectUrl = redirectUrl, FormView = formView });
        protected void RegisterRedirectMode(string mode, string redirectUrl)
            => AuthenticatonModes.Add(mode, new AuthenticationMode { Mode = mode, IsExternalService = true, RedirectUrl=redirectUrl });
        protected void RegisterImediateAuthenticationMode(string mode)
            => AuthenticatonModes.Add(mode, new AuthenticationMode { Mode = mode, ImediatelyAuthenticate = true });

        public IAuthenticationMode GetMode(string mode)
        => AuthenticatonModes.FirstOrDefault(m=> m.Key == mode).Value;
    }
}
