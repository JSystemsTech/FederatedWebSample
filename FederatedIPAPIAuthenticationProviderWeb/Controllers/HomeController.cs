using FederatedIPAPIAuthenticationProviderWeb.Models;
using FederatedIPAuthenticationService.Attributes;
using FederatedIPAuthenticationService.Attributes.Common;
using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Models;
using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.Web;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using Newtonsoft.Json;
using ServiceLayer.DomainLayer.Models.Data;
using ServiceProvider.Configuration;
using ServiceProvider.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebApiClientShared.Web;

namespace FederatedIPAPIAuthenticationProviderWeb.Controllers
{
    
    
    [FederatedProvider]
    [NoCache]
    public class HomeController : BaseController
    {
        protected override string GetDeafaultNamespace() => typeof(HomeController).Namespace;
        protected IApplicationSettings ApplicationSettings => Services.Get<IApplicationSettings>();
        
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

            var vm = new LoginVM() { ConsumingApplicationFederatedApplicationSettings = ConsumingApplicationFederatedApplicationSettings, TestUsers = ConsumingApplicationTestUsers };

            return vm;
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
            return View(GetSessionLoginVM()); 
        }

        public ActionResult PrivacyPolicy()
        {
            if (!ConsumingApplicationFederatedApplicationSettings.RequirePrivacyPolicy)
            {
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptCookies()
        {
            var token = TokenProvider.RenewToken(AuthenticationRequestToken, clms => {
                clms.Add("AcceptedCookies", new string[] { $"{true}" });
            });
            UpdateAuthenticationRequestTokenCookie(token);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public override ActionResult ExternalAuthenticationPostback(string token) => Authenticate(GetSessionLoginVM(), token);
        private ActionResult Authenticate(LoginVM vm, string externalAuthToken = null)
        {
            vm.ConsumingApplicationFederatedApplicationSettings = ConsumingApplicationFederatedApplicationSettings;

            ConsumerApiAuthenticationResponse response;
            if (!string.IsNullOrWhiteSpace(externalAuthToken))
            {
                ICACUser UserData = AuthenticationProviderDomainFacade.GetCACLoginRequestUser(externalAuthToken);
                response = ConsumerAPI.Authenticate(new
                {
                    vm.Username,
                    vm.Password,
                    vm.Email,
                    vm.TestUserGuid,
                    UserData = JsonConvert.SerializeObject(UserData)//presserialize before API serializes to keep it as a string
                });
            }
            else
            {                
                response = ConsumerAPI.Authenticate(new
                {
                    vm.Username,
                    vm.Password,
                    vm.Email,
                    vm.TestUserGuid
                });
            }
            
            
            if (TryValidateAuthentication(response))
            {
                vm.OnAuthenticationMessage = response.Message;
                ViewBag.ShowSpinner = true;
                ViewBag.MainIcon = "fa-user-check";
                return View("ExternalAuthenticationPostback", vm);
            }
            if(vm.Mode == AuthenticationMode.Basic)
            {
                ModelState.AddModelError("Username", "Invalid Username or password");
                ModelState.AddModelError("Password", "Invalid Username or password");
            }
            else if(vm.Mode == AuthenticationMode.Email)
            {
                ModelState.AddModelError("Email", "Invalid Email or password");
                ModelState.AddModelError("Password", "Invalid Email or password");
            }
            vm.TestUsers = ConsumingApplicationTestUsers;
            return View("Index",vm);
        }


        private void ValidateLoginModel(LoginVM vm)
        {
            if(vm.Mode == AuthenticationMode.Test && (FederatedApplicationSettings.IsProductionEnvironment() || ConsumingApplicationFederatedApplicationSettings.IsProductionEnvironment()))
            {
                ModelState.AddModelError("Mode", $"Cannot Use Authenticate with test users in '{FederatedApplicationSettings.SiteEnvironment}' Environment");
            }
            else if (vm.Mode == AuthenticationMode.Basic)
            {
                if (string.IsNullOrEmpty(vm.Username))
                {
                    ModelState.AddModelError("Username", "Username is Required");
                }
                if (string.IsNullOrEmpty(vm.Password))
                {
                    ModelState.AddModelError("Password", "Password is Required");
                }
            }else if (vm.Mode == AuthenticationMode.Email)
            {
                if (string.IsNullOrEmpty(vm.Email))
                {
                    ModelState.AddModelError("Email", "Email is Required");
                }
                try
                {
                    var ValidateEmail = new MailAddress(vm.Email);
                }
                catch
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                }
                if (string.IsNullOrEmpty(vm.Password))
                {
                    ModelState.AddModelError("Password", "Password is Required");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM vm)
        {
            ValidateLoginModel(vm);
            if (!ModelState.IsValid)
            {
                vm.ConsumingApplicationFederatedApplicationSettings = ConsumingApplicationFederatedApplicationSettings;
                vm.TestUsers = ConsumingApplicationTestUsers;
                return View("Index", vm);
            }
            if(vm.Mode == AuthenticationMode.CAC)
            {
                return Redirect(BeginGetExternalAuthenticationPostbackUrl());
            }
            else
            {
                return Authenticate(vm);
            }
            
        }
        [FederatedAllowAnnonomous]
        public ActionResult SessionEnd()
            => View();

        [HttpGet]
        public ActionResult CACService(string returnUrl)
        {
            //var response = MailService.SendMail((msg, content) =>
            //{
            //    msg.TOs = new[] { "jsmc123@hotmail.com" };
            //    msg.FromDisplayName = "No-Reply-FederatedAuth-Test";
            //    msg.Subject = "Test Email from c# project";
            //    content.Content = "this is a test of the c# email system";
            //});
            UriBuilder returnUrlHelper = new UriBuilder(returnUrl);
            Dictionary<string, string> postBackValues = new Dictionary<string, string>();
            (returnUrlHelper.Query.StartsWith("?") ? returnUrlHelper.Query.Remove(0, 1) : returnUrlHelper.Query).Split('&').ToList().ForEach(p =>
            {
                string[] meta = p.Split('=');
                postBackValues.Add(meta[0], HttpUtility.UrlDecode(meta[1]));
            });

            ViewBag.PostBackAction = returnUrlHelper.Uri.GetLeftPart(UriPartial.Path);
            ViewBag.PostBackValues = postBackValues;
            return View();
        }
    }
}