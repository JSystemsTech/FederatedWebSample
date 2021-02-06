using FederatedAuthNAuthZ.Web;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace FederatedAuthNAuthZ.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class FederatedAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        protected AuthenticationContext AuthenticationContext { get; set; }
        protected UrlHelper Url { get; set; }
        protected HttpContextBase HttpContext { get => AuthenticationContext.HttpContext; }
        protected RouteData RouteData { get => AuthenticationContext.RouteData; }
        protected HttpRequestBase Request { get => HttpContext.Request; }
        private bool IsSecureRequest { get => Request.Url.Scheme.ToLower().Trim() == "https"; }
        protected HttpResponseBase Response { get => HttpContext.Response; }
        protected IPrincipal User { get => HttpContext.User; }
        protected IIdentity Identity { get => User == null ? null : User.Identity; }
        protected bool IsAuthenticated { get => Identity != null && Identity.IsAuthenticated; }
        protected bool IsPostRequest { get => Request.HttpMethod == "POST"; }
        public bool IsAuthenticatedRoute { get; private set; }
        protected IRedirectToActionController RedirectToActionController { get; private set; }
        protected string CreateAuthenticationRequestTokenCookieSuffix() => Guid.NewGuid().ToString();
        public FederatedAuthenticationFilter(bool isAuthenticatedRoute = true) : base()
        {
            IsAuthenticatedRoute = isAuthenticatedRoute;
        }
        public virtual void OnAuthentication(AuthenticationContext filterContext) {
            AuthenticationContext = filterContext;
            Url = new UrlHelper(Request.RequestContext);
            RedirectToActionController = AuthenticationContext.Controller is IRedirectToActionController controller ? controller : null;
        }

        public virtual void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext) { }

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

        protected virtual HttpCookie GetHttpCookie(string name) => Request.Cookies.AllKeys.Contains(name) ? Request.Cookies[name] : null;
        protected virtual bool HasValidHttpCookie(string name) => GetHttpCookie(name) is HttpCookie cookie && (cookie.Expires == null || cookie.Expires < DateTime.UtcNow);
        protected virtual HttpCookie CreateCookie(string name, string value, DateTime expires, bool strict = false) => new HttpCookie(name, value) { HttpOnly = true, Expires = expires, SameSite = strict ? SameSiteMode.Strict : SameSiteMode.Lax, Secure = IsSecureRequest };
        protected virtual HttpCookie CreateSessionCookie(string name, string value, bool strict = false) => new HttpCookie(name, value) { HttpOnly = true, SameSite = strict ? SameSiteMode.Strict : SameSiteMode.Lax, Secure = IsSecureRequest };
        
        protected virtual void SetHttpCookie(HttpCookie cookie) => Response.Cookies.Add(cookie);
        protected virtual void RemoveHttpCookie(HttpCookie cookie, bool destroy = false)
        {
            if (cookie != null)
            {
                /*Do not remove cross site cookies unless destroy is set to true*/
                if (cookie.SameSite != SameSiteMode.None || destroy == true)
                {
                    cookie.Expires = DateTime.UtcNow.AddMinutes(-1);
                    cookie.Value = "";
                }
                Response.Cookies.Add(cookie);
            }

        }
        protected void SetAuthenticationRequestTokenCookie(string name, string token, DateTime expiration) => SetHttpCookie(CreateCookie(name, token, expiration));
    }
    
    
}
