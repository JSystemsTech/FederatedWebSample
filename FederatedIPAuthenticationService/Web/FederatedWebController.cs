using FederatedIPAuthenticationService;
using FederatedIPAuthenticationService.Attributes;
using FederatedIPAuthenticationService.Configuration;
using FederatedIPAuthenticationService.Extensions;
using FederatedIPAuthenticationService.Principal;
using FederatedIPAuthenticationService.Services;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using FederatedIPAuthenticationService.Web.SiteMap;
using Newtonsoft.Json;
using ServiceProvider.Web;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApiClientShared.Web
{
    public abstract class FederatedWebController : ServiceProviderController
    {
        protected IFederatedApplicationSettings FederatedApplicationSettings => Services.Get<IFederatedApplicationSettings>();
        protected IMailService MailService => Services.Get<IMailService>();

        protected ITokenProvider TokenProvider => Services.Get<ITokenProvider>();

        protected string LogoutRedirectUrl => HttpContext.Session["LogoutRedirectUrl"] is string redirectUrl && !string.IsNullOrWhiteSpace(redirectUrl) ? redirectUrl : "";


        
        
        
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
        private string AuthenticationRequestCookieName { get => $"{FederatedSettings.FederatedAuthenticationRequestTokenCookiePrefix}{FederatedApplicationSettings.SiteId}"; }
        private HttpCookie GetHttpCookie(string name) => Request.Cookies.AllKeys.Contains(name) ? Request.Cookies[name] : null;
        protected HttpCookie CreateAuthenticationRequestCookie(string value) => new HttpCookie(AuthenticationRequestCookieName, value) { HttpOnly = true, Expires = DateTime.UtcNow.AddMinutes(5), SameSite = SameSiteMode.Lax, Secure = IsSecureRequest };
        protected void RemoveAuthenticationRequestCookie() => Response.Cookies.Add(new HttpCookie(AuthenticationRequestCookieName, "") { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(-1), SameSite = SameSiteMode.Lax, Secure = IsSecureRequest });
        
        private bool IsSecureRequest { get => Request.Url.Scheme.ToLower().Trim() == "https"; }
        

        protected string BuildUrl(string url, IDictionary<string, object> query = null)
        {
            UriBuilder builder = new UriBuilder(url);
            List<string> queryParams = new List<string>();
            (builder.Query.StartsWith("?") ? builder.Query.Remove(0, 1) : builder.Query).Split('&').Where(value => !string.IsNullOrWhiteSpace(value)).ToList().ForEach(p =>
            {
                queryParams.Add(p);
            });
            if (query != null && query.Count() > 0)
            {
                query.Select(parameter => $"{parameter.Key}={HttpUtility.UrlEncode(parameter.Value.ToString())}").ToList().ForEach(p => queryParams.Add(p));
            }
            builder.Query = string.Join("&", queryParams);
            return builder.Uri.ToString();
        }
        protected string BuildUrl<T>(string url, T query)
        {
            UriBuilder builder = new UriBuilder(url);
            List<string> queryParams = new List<string>();
            (builder.Query.StartsWith("?") ? builder.Query.Remove(0, 1) : builder.Query).Split('&').Where(value => !string.IsNullOrWhiteSpace(value)).ToList().ForEach(p =>
            {
                queryParams.Add(p);
            });
            foreach (var prop in query.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                queryParams.Add($"{prop.Name}={HttpUtility.UrlEncode(prop.GetValue(query).ToString())}");
            }
            builder.Query = string.Join("&", queryParams);
            return builder.Uri.ToString();
        }
        protected RedirectResult RedirectToUrl(string url, IDictionary<string, object> query=null)
        => new RedirectResult(BuildUrl(url, query));
        protected RedirectResult RedirectToUrl<T>(string url, T query)
        => new RedirectResult(BuildUrl(url, query));
        protected PartialViewResult PartialFormView<T>(string formViewName, T model) => PartialView($"Forms/{formViewName}", model);

        protected override void OnActionExecuting(ActionExecutingContext filterContext)

        {
            base.OnActionExecuting(filterContext);
            /* only build site Map on PageRequests */
            if (filterContext.ActionDescriptor is ReflectedActionDescriptor method &&
                !typeof(JsonResult).IsAssignableFrom(method.MethodInfo.ReturnType) &&
                !typeof(PartialViewResult).IsAssignableFrom(method.MethodInfo.ReturnType) &&
                !typeof(FileResult).IsAssignableFrom(method.MethodInfo.ReturnType))
            {
                bool IsAuthenticated = HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated;
                IEnumerable<SiteMapArea> areas = SiteMapFactory.BuildSiteMap(GetDeafaultNamespace(), IsAuthenticated, Url);

                var map = new ExpandoObject() as IDictionary<string, object>;
                areas.ToList().ForEach(area => map.Add(area.Name, area.ToMap()));
                string json = JsonConvert.SerializeObject(map);
                ViewBag.SiteMap = json;
            }
            ViewBag.IsAuthenticated = filterContext.HttpContext.User.Identity != null && filterContext.HttpContext.User.Identity.IsAuthenticated;
            ViewBag.SessionTimeout = filterContext.HttpContext.User is FederatedPrincipal principal ? principal.SessionTimeout : 0;
            ViewBag.FederatedApplicationSettings = FederatedApplicationSettings;
            
        }

        protected abstract string GetDeafaultNamespace();

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

    public abstract class FederatedProviderWebController : FederatedWebController
    {
        
        private Uri ExternalAuthenticationUri => GetUri(FederatedApplicationSettings.ExternalAuthenticationtUrl);
        protected IConsumerAuthenticationApi ConsumerAPI { get; private set; }
        protected IConsumerApplicationSettingsResponse ConsumerApplicationSettings { get; private set; }
        protected IFederatedApplicationSettings ConsumingApplicationFederatedApplicationSettings => ConsumerApplicationSettings.FederatedApplicationSettings;
        protected IEnumerable<ConsumerUser> ConsumingApplicationTestUsers => ConsumerApplicationSettings.TestUsers;
        private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        private HttpCookie CreateAuthenticationCookie(string value, DateTime expires) => new HttpCookie(AuthenticationCookieName, value) { HttpOnly = true, Expires = expires, SameSite = SameSiteMode.Lax, Secure = IsSecureRequest };


        private void RefreshAuthenticationRequestTokenCookie()
        {
            string authenticationRequestToken = TokenProvider.CreateToken(claims => { });
            Response.Cookies.Add(CreateAuthenticationRequestCookie(authenticationRequestToken));
        }
        protected string AuthenticationRequestToken => Response.Cookies.Get(AuthenticationRequestCookieName) is HttpCookie httpCookie && !string.IsNullOrWhiteSpace(httpCookie.Value) ? httpCookie.Value : "";
        protected IDictionary<string, IEnumerable<string>> AuthenticationRequestClaims => TokenProvider.GetTokenClaims(AuthenticationRequestToken);
        
        protected bool TryValidateAuthentication(ConsumerApiAuthenticationResponse response)
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

        private string AuthenticationCookieName { get => $"{FederatedSettings.FederatedAuthenticationTokenCookiePrefix}{ConsumingApplicationFederatedApplicationSettings.GetCookieSuffix()}"; }

        [HttpPost]
        [FederatedExternalPostback]
        public abstract ActionResult ExternalAuthenticationPostback(string token);

        protected override void OnActionExecuting(ActionExecutingContext filterContext)

        {
            base.OnActionExecuting(filterContext);

            string ConsumerAuthenticationApiUrl = TokenProvider.GetTokenClaims(AuthenticationRequestToken)["ConsumerAuthenticationApiUrl"].FirstOrDefault();
            ConsumerAPI = new ConsumerAuthenticationApi(EncryptionService, TokenProvider, ConsumerAuthenticationApiUrl);

            ConsumerApplicationSettings = ConsumerAPI.GetConsumerApplicationSettings();
            ViewBag.ConsumingApplicationFederatedApplicationSettings = ConsumingApplicationFederatedApplicationSettings;
            ViewBag.LogoImage = ConsumerApplicationSettings.LogoImage;
        }
        protected abstract string SaveAuthenticationRequest(string authenticationRequestToken);
        protected string BeginGetExternalAuthenticationPostbackUrl()
        {
            string returnUrl = BuildUrl(GetUri(Url.Action("ExternalAuthenticationPostback")).AbsoluteUri, new { AuthenticationRequest = SaveAuthenticationRequest(AuthenticationRequestToken) });
            return BuildUrl(ExternalAuthenticationUri.AbsoluteUri, new { returnUrl = returnUrl });
        }
        private string AuthenticationRequestCookieName { get => $"{FederatedSettings.FederatedAuthenticationRequestTokenCookiePrefix}{FederatedApplicationSettings.SiteId}"; }
        private bool IsSecureRequest { get => Request.Url.Scheme.ToLower().Trim() == "https"; }
        private  HttpCookie CreateCookie(string name, string value, DateTime expires, bool strict = false) => new HttpCookie(name, value) { HttpOnly = true, Expires = expires, SameSite = strict ? SameSiteMode.Strict : SameSiteMode.Lax, Secure = IsSecureRequest };

        private void SetHttpCookie(HttpCookie cookie) => Response.Cookies.Add(cookie);
        
        protected void UpdateAuthenticationRequestTokenCookie(string token) => SetHttpCookie(CreateCookie(AuthenticationRequestCookieName, token, (DateTime)TokenProvider.GetExpirationDate(token)));



    }
}
