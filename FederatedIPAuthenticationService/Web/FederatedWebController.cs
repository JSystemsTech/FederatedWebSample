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
    public abstract class FederatedWebController: ServiceProviderController
    {
        protected IFederatedConsumerSettings FederatedConsumerSettings => Services.Get<IFederatedConsumerSettings>();
        protected IFederatedProviderSettings FederatedProviderSettings => Services.Get<IFederatedProviderSettings>();
        protected ISiteMeta SiteMeta => Services.Get<ISiteMeta>();
        protected IMailService MailService => Services.Get<IMailService>();

        protected ITokenProvider TokenProvider => Services.Get<ITokenProvider>();
       
        protected string LogoutRedirectUrl => HttpContext.Session["LogoutRedirectUrl"] is string redirectUrl && !string.IsNullOrWhiteSpace(redirectUrl) ? redirectUrl: "";
        protected string AuthenticationRequestToken => HttpContext.Session["AuthenticationRequestToken"] is string authenticationRequestToken && !string.IsNullOrWhiteSpace(authenticationRequestToken) ? authenticationRequestToken : "";
        protected ISiteMeta ConsumingApplicationSiteMeta => HttpContext.Session["SiteMeta"] as ISiteMeta;
        
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
        private string AuthenticationRequestCookieName { get => $"{FederatedSettings.FederatedAuthenticationRequestTokenCookiePrefix}{SiteMeta.SiteId}"; }
        protected string GetAuthenticationRequestToken() => GetHttpCookie(AuthenticationRequestCookieName) is HttpCookie cookie?cookie.Value: null ;
        private HttpCookie GetHttpCookie(string name) => Request.Cookies.AllKeys.Contains(name) ? Request.Cookies[name] : null;
        private HttpCookie CreateAuthenticationRequestCookie(string value) => new HttpCookie(AuthenticationRequestCookieName, value) { HttpOnly = true, Expires = DateTime.UtcNow.AddMinutes(5), SameSite = SameSiteMode.Lax, Secure = IsSecureRequest };
        private void RemoveAuthenticationRequestCookie() => Response.Cookies.Add(new HttpCookie(AuthenticationRequestCookieName, "") { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(-1), SameSite = SameSiteMode.Lax, Secure = IsSecureRequest });
        private void RefreshAuthenticationRequestTokenCookie()
        {
            string authenticationRequestToken = TokenProvider.CreateToken(claims => {
                foreach (KeyValuePair<string, string> siteMetaItem in ConsumingApplicationSiteMeta.Collection)
                {
                    claims.AddUpdate(siteMetaItem.Key, siteMetaItem.Value.ToString());
                }
            });
            Response.Cookies.Add(CreateAuthenticationRequestCookie(authenticationRequestToken));
        }

        protected ConsumerApiAuthenticateResponse CreateConsumerAPISuccessResponse(ConsumerUser authenticatedUser)
        {
            var token = TokenProvider.CreateToken(authTokenClaims => {
                authTokenClaims.AddUpdate("UserGuid", authenticatedUser.Guid.ToString());
                foreach (KeyValuePair<string, string> siteMetaItem in SiteMeta.Collection)
                {
                    authTokenClaims.AddUpdate(siteMetaItem.Key, siteMetaItem.Value.ToString());
                }
            });
            return new ConsumerApiAuthenticateResponse() {
                AuthenticationToken = token,
                Message = $"Logging in {authenticatedUser.Name}",
                AuthenticationTokenExpiration = TokenProvider.GetExpirationDate(token)
            };            
        }
        private bool ConsumingApplicationUseRhealm { get => !string.IsNullOrWhiteSpace(ConsumingApplicationSiteMeta.SiteRhealmId); }
        private string AuthenticationCookieName { get => $"{FederatedSettings.FederatedAuthenticationTokenCookiePrefix}{(ConsumingApplicationUseRhealm ? ConsumingApplicationSiteMeta.SiteRhealmId : ConsumingApplicationSiteMeta.SiteId)}"; }
        private bool IsSecureRequest { get => Request.Url.Scheme.ToLower().Trim() == "https"; }
        private HttpCookie CreateAuthenticationCookie(string value, DateTime expires) => new HttpCookie(AuthenticationCookieName, value) { HttpOnly = true, Expires = expires, SameSite = SameSiteMode.Lax, Secure = IsSecureRequest };
        protected bool TryValidateAuthentication(ConsumerApiAuthenticateResponse response)
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
            ViewBag.SiteMeta = SiteMeta;
            ViewBag.ConsumingApplicationSiteMeta = ConsumingApplicationSiteMeta;
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
        
        private Uri ExternalAuthenticationUri => GetUri(FederatedProviderSettings.ExternalAuthenticationtUrl);

        [HttpPost]
        [FederatedExternalPostback]
        public abstract ActionResult ExternalAuthenticationPostback(string token);

        protected abstract string SaveAuthenticationRequest(string authenticationRequestToken);
        protected string BeginGetExternalAuthenticationPostbackUrl()
        {
            string authenticationRequestToken = HttpContext.Session["AuthenticationRequestToken"] as string;
            string returnUrl = BuildUrl(GetUri(Url.Action("ExternalAuthenticationPostback")).AbsoluteUri, new { AuthenticationRequest = SaveAuthenticationRequest(authenticationRequestToken) });
            return BuildUrl(ExternalAuthenticationUri.AbsoluteUri, new { returnUrl = returnUrl });
        }
    }
}
