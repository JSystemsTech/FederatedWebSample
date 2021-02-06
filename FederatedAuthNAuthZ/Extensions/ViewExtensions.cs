using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FederatedAuthNAuthZ.Extensions
{
    public class ViewExtensions
    {
        public static T CreateController<T>(RouteData routeData = null) where T : Controller, new()
        {
            // create a disconnected controller instance
            T controller = new T();

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper;
            if (System.Web.HttpContext.Current != null)
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            else
                throw new InvalidOperationException("Can't create Controller Context if no active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name.ToLower().Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }
        public static string RenderViewToString<T>(string viewName, object model, Action<dynamic> viewBagHandler) where T : Controller, new()
        {
            var controller = CreateController<T>();
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            ViewDataDictionary viewData = new ViewDataDictionary(model);
            
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewBagHandler(viewContext.ViewBag);
                viewResult.View.Render(viewContext, sw);
                
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
