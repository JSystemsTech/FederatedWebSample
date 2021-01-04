using FederatedIPAuthenticationService.Attributes.Common;
using ServiceLayer.AuthNAuthZ;
using System.Security.Principal;
using System.Web.Mvc;
using WebApiClientShared.Web;

namespace FederatedIPAPIConsumer.Controllers
{

    [FederatedAuthorize]
    public abstract class BaseController : FederatedWebController
    {
        [FederatedAllowAnnonomous]
        public ActionResult Logout() {
            ViewBag.Logout = true;
            ViewBag.RedirectUrl = LogoutRedirectUrl;
            return View(SiteMeta);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RenewSession()
        {
            return JsonResponse(new { test= 8 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeCss(string cssMode)
        {
            HttpContext.Session["CssMode"] = cssMode;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        protected override string GetDeafaultNamespace() => typeof(HomeController).Namespace;
        protected string CssMode => HttpContext.Session["CssMode"] is string cssMode && !string.IsNullOrWhiteSpace(cssMode) ? cssMode : "Default";
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.CssMode = CssMode;
            ViewBag.CurrentUserName = User.Identity is IIdentity userIdentity ? userIdentity.Name : ""; 
        }
    }
}