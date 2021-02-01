using FederatedAuthNAuthZ.Attributes;
using FederatedAuthNAuthZ.Services;
using Newtonsoft.Json;
using ServiceLayer.WebHelpers.SiteMap;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using WebApiClientShared.Web;

namespace FederatedIPAPIConsumer.Controllers
{


    [NoCache]
    public abstract class BaseController : FederatedApplicationWebController
    {
        private ICssThemeService CssThemeService => Services.Get<ICssThemeService>();
        [FederatedApplication(false)]
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
        protected ThemeBundle Theme => CssThemeService.GetTheme(HttpContext.Session["Theme"] is string theme && !string.IsNullOrWhiteSpace(theme) ? theme : FederatedApplicationSettings.Theme);
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.CurrentUserName = User.Identity is IIdentity userIdentity ? userIdentity.Name : "";
            ViewBag.ThemeBundle = Theme.Path;
            ViewBag.IsDarkMode = Theme.IsDarkTheme;/* only build site Map on PageRequests */
            if (filterContext.ActionDescriptor is ReflectedActionDescriptor method &&
                !typeof(JsonResult).IsAssignableFrom(method.MethodInfo.ReturnType) &&
                !typeof(PartialViewResult).IsAssignableFrom(method.MethodInfo.ReturnType) &&
                !typeof(FileResult).IsAssignableFrom(method.MethodInfo.ReturnType))
            {
                bool IsAuthenticated = HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated;
                IEnumerable<SiteMapArea> areas = SiteMapFactory.BuildSiteMap(typeof(HomeController).Namespace, IsAuthenticated, Url);

                var map = new ExpandoObject() as IDictionary<string, object>;
                areas.ToList().ForEach(area => map.Add(area.Name, area.ToMap()));
                string json = JsonConvert.SerializeObject(map);
                ViewBag.SiteMap = json;
            }
        }
        
    }
}