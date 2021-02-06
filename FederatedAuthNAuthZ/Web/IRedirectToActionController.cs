using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace FederatedAuthNAuthZ.Web
{
    public interface IRedirectToActionController
    {
        RedirectToRouteResult RedirectResultToAction(string actionName);
        RedirectToRouteResult RedirectResultToAction(string actionName, object routeValues);
        RedirectToRouteResult RedirectResultToAction(string actionName, RouteValueDictionary routeValues);
        RedirectToRouteResult RedirectResultToAction(string actionName, string controllerName);
        RedirectToRouteResult RedirectResultToAction(string actionName, string controllerName, object routeValues);
    }
}
