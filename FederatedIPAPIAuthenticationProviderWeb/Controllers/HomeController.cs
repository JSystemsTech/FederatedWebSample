using FederatedIPAPIAuthenticationProviderWeb.Models;
using FederatedIPAuthenticationService.Attributes;
using FederatedIPAuthenticationService.Attributes.Common;
using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Models;
using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.Web;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using ServiceProvider.Configuration;
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
    public class HomeController : FederatedProviderWebController
    {
        protected override string GetDeafaultNamespace() => typeof(HomeController).Namespace;
        protected IApplicationSettings ApplicationSettings => Services.Get<IApplicationSettings>();
        private IAuthenticationRequestCache AuthenticationRequestCache { get => Services.Get<IAuthenticationRequestCache>(); }
        private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        protected bool AcknowlagedPrivacyNotice => AuthenticationRequestClaims.ContainsKey("AcknowlagedPrivacyNotice") && 
            bool.TryParse(AuthenticationRequestClaims["AcknowlagedPrivacyNotice"].FirstOrDefault(), out bool acknowlagedPrivacyNotice) ? acknowlagedPrivacyNotice: false;

        private LoginVM GetSessionLoginVM() {

            var vm = new LoginVM() { ConsumingApplicationSiteMeta = ConsumingApplicationSiteMeta, TestUsers = ConsumerAPI.GetTestUsers() };

            return vm;
        }
        public ActionResult Index()
        { 
            if (ConsumingApplicationSiteMeta.RequirePrivacyNotice && !AcknowlagedPrivacyNotice) {
                return RedirectToAction("PrivacyNotice"); 
            } 
            return View(GetSessionLoginVM()); 
        }

        public ActionResult PrivacyNotice()
        {
            if (!ConsumingApplicationSiteMeta.RequirePrivacyNotice)
            {
                return RedirectToAction("Index");
            }
            var vm = new LoginVM() { PrivacyNotice = ConsumerAPI.GetPrivacyNotice()};
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivacyNoticeAckowlage()
        {
            var token = TokenProvider.RenewToken(AuthenticationRequestToken,clms => {
                clms.Add("AcknowlagedPrivacyNotice", new string[] { $"{true}" });
            });
            UpdateAuthenticationRequestTokenCookie(token);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public override ActionResult ExternalAuthenticationPostback(string token) => Authenticate(GetSessionLoginVM(), token);
        private ActionResult Authenticate(LoginVM vm, string externalAuthToken = null)
        {
            vm.ConsumingApplicationSiteMeta = ConsumingApplicationSiteMeta;
            
            
            string LoginModelToken = TokenProvider.RenewToken(AuthenticationRequestToken, claims => {                
                claims.AddUpdate("providerAuthenticationCredentials", ProviderAuthenticationCredentials.BuildProviderAuthenticationCredentialsData(vm.Username, vm.Email, vm.Password, vm.TestUserGuid).Select(i => i != null ? i.ToString(): null));
                if (!string.IsNullOrWhiteSpace(externalAuthToken))
                {
                    IEnumerable<string> externalAuthAuthorizedUser = new object[] { externalAuthToken, 1234567890 }.Select(i => i != null ? i.ToString() : null); /*this would be the get login user CAC service call*/
                    claims.AddUpdate("externalAuthAuthorizedUser", externalAuthAuthorizedUser);
                }
            });
            ConsumerApiAuthenticationResponse response = ConsumerAPI.Authenticate(LoginModelToken);
            if (TryValidateAuthentication(response))
            {
                vm.OnAuthenticationMessage = response.Message;
                ViewBag.ShowSpinner = true;
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
            vm.TestUsers = ConsumerAPI.GetTestUsers();
            return View("Index",vm);
        }


        private void ValidateLoginModel(LoginVM vm)
        {
            if (vm.Mode == AuthenticationMode.Basic)
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

        protected override string SaveAuthenticationRequest(string authenticationRequestToken) {
            Guid authenticationRequestGuid = AuthenticationRequestCache.Add(authenticationRequestToken);
            return EncryptionService.Encrypt(authenticationRequestGuid.ToString());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM vm)
        {
            ValidateLoginModel(vm);
            if (!ModelState.IsValid)
            {
                vm.ConsumingApplicationSiteMeta = ConsumingApplicationSiteMeta;
                vm.TestUsers = ConsumerAPI.GetTestUsers();
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