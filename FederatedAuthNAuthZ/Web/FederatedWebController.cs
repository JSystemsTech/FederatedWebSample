using FederatedAuthNAuthZ.Attributes;
using FederatedAuthNAuthZ.Configuration;
using FederatedAuthNAuthZ.Principal;
using FederatedAuthNAuthZ.Services;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using ServiceProvider.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FederatedAuthNAuthZ.Web
{
    public abstract class FederatedWebController : ServiceProviderController,IRedirectToActionController
    {
        protected IFederatedApplicationSettings FederatedApplicationSettings => Services.Get<IFederatedApplicationSettings>();

        protected ITokenProvider TokenProvider => Services.Get<ITokenProvider>();

        protected string LogoutRedirectUrl => HttpContext.Session["LogoutRedirectUrl"] is string redirectUrl && !string.IsNullOrWhiteSpace(redirectUrl) ? redirectUrl : "";

        public RedirectToRouteResult RedirectResultToAction(string actionName) => RedirectToAction(actionName);
        public RedirectToRouteResult RedirectResultToAction(string actionName, object routeValues) => RedirectToAction(actionName, routeValues);
        public RedirectToRouteResult RedirectResultToAction(string actionName, RouteValueDictionary routeValues) => RedirectToAction(actionName, routeValues);
        public RedirectToRouteResult RedirectResultToAction(string actionName, string controllerName) => RedirectToAction(actionName, controllerName);
        public RedirectToRouteResult RedirectResultToAction(string actionName, string controllerName, object routeValues) => RedirectToAction(actionName, controllerName, routeValues);



        private object GetJsonResponseData(object data) => new { sessionTimeout = HttpContext.User is FederatedPrincipal principal ? principal.SessionTimeout : 0, data };
        protected JsonResult JsonResponse(object data, string contentType, Encoding contentEncoding)
        => Json(GetJsonResponseData(data), contentType, contentEncoding);
        protected JsonResult JsonResponse(object data, JsonRequestBehavior behavior)
        => Json(GetJsonResponseData(data), behavior);
        protected JsonResult JsonResponse(object data, string contentType, JsonRequestBehavior behavior)
        => Json(GetJsonResponseData(data), contentType, behavior);
        protected JsonResult JsonResponse(object data)
        => Json(GetJsonResponseData(data));


        protected new JsonResult Json(object data)
        {
            var sessionTimeout = HttpContext.User is FederatedPrincipal principal ? principal.SessionTimeout : 0;
            return Json(new { sessionTimeout, data }, JsonRequestBehavior.AllowGet);
        }
        private string AuthenticationRequestCookieName { get => $"{FederatedApplicationSettings.GetAuthRequestCookieName()}{AuthenticationRequestTokenCookieSuffix}"; }
        private string AuthenticationRequestTokenCookieSuffix => HttpContext.Session["AuthenticationRequestTokenCookieSuffix"].ToString();


        protected HttpCookie CreateAuthenticationRequestCookie(string value) => new HttpCookie(AuthenticationRequestCookieName, value) { HttpOnly = true, Expires = DateTime.UtcNow.AddMinutes(5), SameSite = SameSiteMode.Lax, Secure = IsSecureRequest };
        protected void RemoveAuthenticationRequestCookie() => Response.Cookies.Add(new HttpCookie(AuthenticationRequestCookieName, "") { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(-1), SameSite = SameSiteMode.Lax, Secure = IsSecureRequest });
        
        private bool IsSecureRequest { get => Request.Url.Scheme.ToLower().Trim() == "https"; }
        
        protected string BuildUrl(string url, IDictionary<string, object> query)
        {
            UriBuilder builder = new UriBuilder(url);
            List<string> queryParams = new List<string>();
            (builder.Query.StartsWith("?") ? builder.Query.Remove(0, 1) : builder.Query).Split('&').Where(value => !string.IsNullOrWhiteSpace(value)).ToList().ForEach(p =>
            {
                queryParams.Add(p);
            });
            foreach (var prop in query)
            {
                queryParams.Add($"{prop.Key}={HttpUtility.UrlEncode(prop.Value.ToString())}");
            }
            builder.Query = string.Join("&", queryParams);
            return builder.Uri.ToString();
        }
        protected PartialViewResult PartialFormView<T>(string formViewName, T model) => PartialView($"Forms/{formViewName}", model);

        protected override void OnAuthorization(AuthorizationContext filterContext)

        {
            base.OnAuthorization(filterContext);
            
            ViewBag.IsAuthenticated = filterContext.HttpContext.User.Identity != null && filterContext.HttpContext.User.Identity.IsAuthenticated;
            ViewBag.SessionTimeout = filterContext.HttpContext.User is FederatedPrincipal principal ? principal.SessionTimeout : 0;
            ViewBag.FederatedApplicationSettings = FederatedApplicationSettings;
            
        }

        protected Uri GetUri(string absoluteOrRelativeUrl)
        {
            try
            {
                return new Uri(absoluteOrRelativeUrl);
            }
            catch
            {
                return new Uri($"{Request.Url.Scheme}://{Request.Url.Authority}{VirtualPathUtility.ToAbsolute(absoluteOrRelativeUrl)}");
            }
        }

    }
    [FederatedApplication]
    public abstract class FederatedApplicationWebController : FederatedWebController { }
    
    [FederatedAuthenticationProvider]
    public abstract class FederatedProviderWebController : FederatedWebController
    {
        
        protected IApplicationAuthenticationAPI ApplicationAuthenticationAPI { get; private set; }
        protected IApplicationAuthenticationAPIApplicationSettingsResponse TargetApplicationSettings { get; private set; }
        protected IFederatedApplicationSettings ConsumingApplicationFederatedApplicationSettings => TargetApplicationSettings.FederatedApplicationSettings;
        protected IProviderAuthenticationModeService ProviderAuthenticationModeService => Services.Get<IProviderAuthenticationModeService>();
        protected IEnumerable<ApplicationUser> ConsumingApplicationTestUsers => TargetApplicationSettings.TestUsers;
        private HttpCookie CreateAuthenticationCookie(string value, DateTime expires) => new HttpCookie(AuthenticationCookieName, value) { HttpOnly = true, Expires = expires, SameSite = SameSiteMode.Lax, Secure = IsSecureRequest };


        private void RefreshAuthenticationRequestTokenCookie()
        {
            string authenticationRequestToken = TokenProvider.CreateToken(claims => { });
            Response.Cookies.Add(CreateAuthenticationRequestCookie(authenticationRequestToken));
        }
        protected string AuthenticationRequestToken => Response.Cookies.Get(AuthenticationRequestCookieName) is HttpCookie httpCookie && !string.IsNullOrWhiteSpace(httpCookie.Value) ? httpCookie.Value : "";
        protected IDictionary<string, IEnumerable<string>> AuthenticationRequestClaims => TokenProvider.GetTokenClaims(AuthenticationRequestToken);
        
        protected bool TryValidateAuthentication(ApplicationAuthenticationApiAuthenticationResponse response)
        {
            if (TokenProvider.IsValidToken(response.AuthenticationToken) && TokenProvider.GetExpirationDate(response.AuthenticationToken) is DateTime expires)
            {
                Response.Cookies.Add(CreateAuthenticationCookie(response.AuthenticationToken, expires));
                RemoveAuthenticationRequestCookie();
                return true;
            }
            else
            {
                RefreshAuthenticationRequestTokenCookie();
                return false;
            }
        }

        private string AuthenticationCookieName { get => $"{ConsumingApplicationFederatedApplicationSettings.GetCookiePrefix()}{ConsumingApplicationFederatedApplicationSettings.GetCookieSuffix()}"; }

        

        protected override void OnAuthorization(AuthorizationContext filterContext)

        {
            base.OnAuthorization(filterContext);
            string ConsumerAuthenticationApiUrl = TokenProvider.GetTokenClaims(AuthenticationRequestToken)["ConsumerAuthenticationApiUrl"].FirstOrDefault();
            ApplicationAuthenticationAPI = new ApplicationAuthenticationAPI(ConsumerAuthenticationApiUrl);

            TargetApplicationSettings = ApplicationAuthenticationAPI.GetApplicationSettings();
            ViewBag.TargetApplicationSettings = TargetApplicationSettings;
            ViewBag.ConsumingApplicationFederatedApplicationSettings = ConsumingApplicationFederatedApplicationSettings;
            ViewBag.LogoImage = TargetApplicationSettings.LogoImage;
        }
        private string GetRedirectUrl(string redirectUrl, string returnAction,IDictionary<string, object> returnQueryParams = null)
        {
            IDictionary<string, object> queryParams = returnQueryParams != null ? returnQueryParams : new Dictionary<string, object>();
            queryParams.Add("api", ConsumingApplicationFederatedApplicationSettings.ConsumerAuthenticationApiUrl.Encrypt());
            string returnUrl = BuildUrl(GetUri(Url.Action(returnAction)).AbsoluteUri, queryParams);
            return BuildUrl(GetUri(redirectUrl).AbsoluteUri, new Dictionary<string, object>() { { "returnUrl", returnUrl } });
        }
        protected RedirectResult RedirectToUrl(string redirectUrl, string returnAction, IDictionary<string, object> returnQueryParams = null) 
            => Redirect(GetRedirectUrl(redirectUrl, returnAction, returnQueryParams));
        private string AuthenticationRequestCookieName { get => $"{FederatedApplicationSettings.GetAuthRequestCookieName()}{AuthenticationRequestTokenCookieSuffix}"; }
        private string AuthenticationRequestTokenCookieSuffix => HttpContext.Session["AuthenticationRequestTokenCookieSuffix"].ToString();
        private bool IsSecureRequest { get => Request.Url.Scheme.ToLower().Trim() == "https"; }
        private  HttpCookie CreateCookie(string name, string value, DateTime expires, bool strict = false) => new HttpCookie(name, value) { HttpOnly = true, Expires = expires, SameSite = strict ? SameSiteMode.Strict : SameSiteMode.Lax, Secure = IsSecureRequest };

        private void SetHttpCookie(HttpCookie cookie) => Response.Cookies.Add(cookie);
        
        protected void UpdateAuthenticationRequestTokenCookie(string token) => SetHttpCookie(CreateCookie(AuthenticationRequestCookieName, token, (DateTime)TokenProvider.GetExpirationDate(token)));



    }
}
