using FederatedAuthNAuthZ.Services;
using ServiceProviderShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FederatedIPAPIAuthenticationProviderWeb.Controllers
{
    public class ProviderController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.ThemeBundle = ServiceManager.GetService<ICssThemeService>().GetTheme("Flatly").Path;
        }
        public ActionResult SessionEnd() => View();

        [HttpGet]
        public ActionResult FakeCACService(string returnUrl)
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