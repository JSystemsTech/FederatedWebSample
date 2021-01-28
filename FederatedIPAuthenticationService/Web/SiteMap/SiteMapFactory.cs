using FederatedAuthNAuthZ.Attributes;
using FederatedAuthNAuthZ.Web.ConsumerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace FederatedAuthNAuthZ.Web.SiteMap
{
    internal class SiteMapFactory
    {
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }
        private static IEnumerable<SiteMapAction> GetListOfAction(Type controller, bool IsAuthenticated)
        {
            var navItems = new List<SiteMapAction>();

            // Get a descriptor of this controller
            ReflectedControllerDescriptor controllerDesc = new ReflectedControllerDescriptor(controller);
            
            bool IsAuthenticatedControllerRoute = controllerDesc.GetCustomAttributes(true).Any(filter => filter is FederatedAuthenticationFilter federatedAuthAttribute && federatedAuthAttribute.IsAuthenticatedRoute);
            // Look at each action in the controller
            foreach (ActionDescriptor action in controllerDesc.GetCanonicalActions())
            {
                bool validAction = true;
                bool isHttpPost = false;
                bool returnsJson = false;
                bool returnsPartialView = false;

                // Get any attributes (filters) on the action
                object[] attributes = action.GetCustomAttributes(false);
                bool IsAuthenticatedRoute = attributes.Any(filter => filter is FederatedAuthenticationFilter federatedAuthAttribute && federatedAuthAttribute.IsAuthenticatedRoute);
                bool IsUnAuthenticatedRoute = attributes.Any(filter => filter is FederatedAuthenticationFilter federatedAuthAttribute && !federatedAuthAttribute.IsAuthenticatedRoute);
                // Look at each attribute

                if (!IsAuthenticated && (IsAuthenticatedControllerRoute || IsAuthenticatedRoute) && !IsUnAuthenticatedRoute)
                {
                    validAction = false;
                }
                else
                {
                    foreach (object filter in attributes)
                    {
                        // Can we navigate to the action?
                        if (filter is ChildActionOnlyAttribute)
                        {
                            validAction = false;
                            break;
                        }
                        if (filter is System.Web.Mvc.HttpGetAttribute)
                        {
                            isHttpPost = false;
                            break;
                        }
                        if (filter is System.Web.Mvc.HttpPostAttribute)
                        {
                            isHttpPost = true;
                        }

                    }
                }

                if (action is ReflectedActionDescriptor method)
                {
                    if (typeof(JsonResult).IsAssignableFrom(method.MethodInfo.ReturnType))
                    {
                        returnsJson = true;
                    }
                    else if (typeof(PartialViewResult).IsAssignableFrom(method.MethodInfo.ReturnType))
                    {
                        returnsPartialView = true;
                    }
                }

                // Add the action to the list if it's "valid"
                if (validAction)
                    navItems.Add(new SiteMapAction()
                    {
                        Name = action.ActionName,
                        IsHttpPost = isHttpPost,
                        AjaxMethod = isHttpPost ? "POST" : "GET",
                        ReturnsJson = returnsJson,
                        ReturnsPartialView = returnsPartialView,
                        IsLink = !returnsJson && !returnsPartialView
                    });
            }
            return navItems;
        }
        public static IEnumerable<SiteMapArea> BuildSiteMap(string defaultNamespace, bool IsAuthenticated, UrlHelper Url)
        {
            var list = GetSubClasses<Controller>().Where(c => !c.IsAbstract && !typeof(ApiController).IsAssignableFrom(c)); /*ignore base classes*/


            // Get all controllers with their actions
            var getAllcontrollers = list.Select(item => new SiteMapController()
            {
                Name = item.Name.Replace("Controller", ""),
                Namespace = item.Namespace,
                MyActions = GetListOfAction(item, IsAuthenticated)
            }).ToList();

            // Now we will get all areas that has been registered in route collection
            var areaMap = RouteTable.Routes.OfType<Route>()
                .Where(d => d.DataTokens != null && d.DataTokens.ContainsKey("area"))
                .Select(
                    r =>
                        new SiteMapArea
                        {
                            SiteMapAreaType = SiteMapAreaType.Area,
                            Name = r.DataTokens["area"].ToString(),
                            Namespace = r.DataTokens["Namespaces"] as IList<string>,
                        }).ToList()
                .Distinct().ToList();


            // Add a new area for default controllers
            areaMap.Insert(0, new SiteMapArea()
            {
                SiteMapAreaType = SiteMapAreaType.Default,
                Name = "Default",
                Namespace = new List<string>() { defaultNamespace }
            });


            foreach (var area in areaMap)
            {
                var temp = new List<SiteMapController>();
                foreach (var item in area.Namespace)
                {
                    var areaControllers = getAllcontrollers.Where(x => x.Namespace == item).ToList();
                    areaControllers.ForEach(c => c.MyActions.ToList().ForEach(a => a.SetUrl(Url, area, c)));



                    temp.AddRange(areaControllers);
                }
                area.SiteMapControllers = temp;
            }

            return areaMap;
        }
    }
}
