using FederatedIPAuthenticationService.Attributes;
using FederatedIPAuthenticationService.Attributes.Common;
using FederatedIPAuthenticationService.Services;
using ServiceLayer.AuthNAuthZ;
using System.Security.Principal;
using System.Web.Mvc;
using WebApiClientShared.Web;

namespace FederatedIPAPIConsumer.Controllers
{

    [FederatedAuthorize]
    [NoCache]
    public abstract class BaseController : FederatedWebController
    {
        private ICssThemeService CssThemeService => Services.Get<ICssThemeService>();
        [FederatedAllowAnnonomous]
        public ActionResult Logout() {
            ViewBag.Logout = true;
            ViewBag.RedirectUrl = LogoutRedirectUrl;
            return View(FederatedApplicationSettings);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RenewSession()
        {
            return JsonResponse(new { test= 8 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeTheme(string theme)
        {
            HttpContext.Session["Theme"] = theme;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        protected override string GetDeafaultNamespace() => typeof(HomeController).Namespace;
        protected ThemeBundle Theme => CssThemeService.GetTheme(HttpContext.Session["Theme"] is string theme && !string.IsNullOrWhiteSpace(theme) ? theme : FederatedApplicationSettings.Theme);
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.CurrentUserName = User.Identity is IIdentity userIdentity ? userIdentity.Name : "";
            ViewBag.ThemeBundle = Theme.Path;
            ViewBag.IsDarkMode = Theme.IsDarkTheme;
        }
        
    }
}