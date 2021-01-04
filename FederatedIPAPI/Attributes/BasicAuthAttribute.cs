using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FederatedIPAPI.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using FederatedIPAPI.Services;

namespace FederatedIPAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : Microsoft.AspNetCore.Mvc.TypeFilterAttribute
    {
        public BasicAuthAttribute(string realm = @"My Realm") : base(typeof(BasicAuth.BasicAuthFilter))
        {
            Arguments = new object[] { realm };
        }
    }


    public class MyTrackingActionFilter : ActionFilterAttribute, IExceptionFilter
    {

        private IJwtSettings JwtSettings { get; set; }
        private IUserService UserService { get; set; }
        public MyTrackingActionFilter(IOptions<JwtSettings> jwtSettings, IUserService userService)
        {
            JwtSettings = jwtSettings.Value;
            UserService = userService;
        }

        public void OnException(ExceptionContext filterContext) { }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            bool isSuccessfulResponse = filterContext.HttpContext.Response.StatusCode >= 200 && filterContext.HttpContext.Response.StatusCode < 300;
            if (isSuccessfulResponse)
            {
                Guid? guid = JwtSettings.GetClientGuid(filterContext.HttpContext.Request.Headers);
                if(guid is Guid clientGuid)
                {
                    filterContext.HttpContext.Response.Headers.Add("Token", JwtSettings.CreateJwtToken(clientGuid));
                }                
            }            
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            Guid? guid = JwtSettings.GetClientGuid(filterContext.HttpContext.Request.Headers);
            if (guid is Guid clientGuid)
            {
                string ConnectedUser = UserService.GetUser(clientGuid).Name;
            }
        }
    }
}
