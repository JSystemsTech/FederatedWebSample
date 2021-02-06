using FederatedAuthNAuthZ.Attributes;
using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using FederatedIPAPIAuthenticationProviderWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FederatedIPAPIAuthenticationProviderWeb.Controllers
{



    [NoCache]
    public class HomeController : FederatedProviderWebController
    {
        
        private ICssThemeService CssThemeService => Services.Get<ICssThemeService>();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.ThemeBundle = CssThemeService.GetTheme(ConsumingApplicationFederatedApplicationSettings.Theme).Path;
        }
        protected bool AcknowlagedPrivacyNotice => AuthenticationRequestClaims.ContainsKey("AcknowlagedPrivacyNotice") && 
            bool.TryParse(AuthenticationRequestClaims["AcknowlagedPrivacyNotice"].FirstOrDefault(), out bool acknowlagedPrivacyNotice) ? acknowlagedPrivacyNotice: false;
        protected bool AcceptedCookies => AuthenticationRequestClaims.ContainsKey("AcceptedCookies") &&
            bool.TryParse(AuthenticationRequestClaims["AcceptedCookies"].FirstOrDefault(), out bool acceptedCookies) ? acceptedCookies : false;

        private LoginVM GetSessionLoginVM() {
            IEnumerable<IAuthenticationMode> FormViewModes = 
                ProviderAuthenticationModeService.GetFormViewModes(ApplicationAuthenticationAPI, ConsumingApplicationFederatedApplicationSettings.AuthenticationModes);

            return new LoginVM() { FormViewModes= FormViewModes };
        }
        
        public ActionResult Index()
        { 
            if (!AcceptedCookies) {
                return RedirectToAction("CookiePolicy"); 
            }
            if (ConsumingApplicationFederatedApplicationSettings.RequirePrivacyPolicy && !AcknowlagedPrivacyNotice)
            {
                return RedirectToAction("PrivacyPolicy");
            }
            IEnumerable<IAuthenticationMode> RedirectOnlyModes = ProviderAuthenticationModeService.GetRedirectOnlyModes(ApplicationAuthenticationAPI, ConsumingApplicationFederatedApplicationSettings.AuthenticationModes);
            
            if (RedirectOnlyModes.FirstOrDefault() is IAuthenticationMode externalAuth)
            {
                RemoveAuthenticationRequestCookie();
                return RedirectToCACService(externalAuth.Mode);
            }
            return View(GetSessionLoginVM()); 
        }

        [ChildActionOnly]
        public PartialViewResult CACForm()
        {
            return PartialView(new LoginModeFormVM() { Mode = "CAC" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CACForm(LoginModeFormVM vm)
        {
            vm.Validate(ModelState);
            if (ModelState.IsValid)
            {
                RemoveAuthenticationRequestCookie();
                return RedirectToCACService(vm.Mode);
            }
            return View("Index", GetSessionLoginVM());
        }

        [ChildActionOnly]
        public PartialViewResult TestUsersForm()
        {
            return PartialView(new TestUserVM() { Mode = "Test", TestUsers = ConsumingApplicationTestUsers });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestUsersForm(TestUserVM vm)
        {
            vm.Validate(ModelState);
            if (ModelState.IsValid)
            {
                return Authenticate(vm.Mode, vm, ReturnToIndex);
            }
            return ReturnToIndex();
        }



        public ActionResult PrivacyPolicy()
        {
            if (!ConsumingApplicationFederatedApplicationSettings.RequirePrivacyPolicy)
            {
                return ReturnToIndex();
            }
            ViewBag.DisplayAsWarning = true;
            ViewBag.MainIcon = "fa-exclamation-triangle";
            return View();
        }
        public ActionResult CookiePolicy()
        {
            ViewBag.MainIcon = "fa-exclamation";
            ViewBag.DisplayAsWarning = true;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivacyPolicyAckowlage()
        {
            var token = TokenProvider.RenewToken(AuthenticationRequestToken,clms => {
                clms.Add("AcknowlagedPrivacyNotice", new string[] { $"{true}" });
            });
            UpdateAuthenticationRequestTokenCookie(token);
            return ReturnToIndex();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptCookies()
        {
            var token = TokenProvider.RenewToken(AuthenticationRequestToken, clms => {
                clms.Add("AcceptedCookies", new string[] { $"{true}" });
            });
            UpdateAuthenticationRequestTokenCookie(token);
            return ReturnToIndex();
        }

        [HttpPost]
        [FederatedAuthenticationProvider(true, true)]
        public ActionResult CACServicePostback(string token, string authMode) => Authenticate(authMode, token, ReturnToIndex);
        private RedirectResult RedirectToCACService(string authMode = "CAC") 
            => RedirectToUrl(ProviderAuthenticationModeService.GetMode(authMode).RedirectUrl, "CACServicePostback", new Dictionary<string, object> { { "authMode", authMode } });

        

        private ActionResult ReturnToIndex() => RedirectToAction("Index");
        private ActionResult Authenticate(string mode, object values, Func<ActionResult> callback)
        {
            ApplicationAuthenticationApiAuthenticationResponse response = ProviderAuthenticationModeService.Authenticate(mode, ApplicationAuthenticationAPI, values);
            var vm = GetSessionLoginVM();
            if (TryValidateAuthentication(response))
            {
                vm.OnAuthenticationMessage = response.Message;
                ViewBag.ShowSpinner = true;
                ViewBag.MainIcon = "fa-user-check";
                return View("ExternalAuthenticationPostback", vm);
            }
            return callback();
        }
    }

}