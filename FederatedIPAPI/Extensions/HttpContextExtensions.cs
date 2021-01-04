using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FederatedIPAPI.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid? GetClientGuid(this Microsoft.AspNetCore.Http.HttpContext context)
        {
            string value = ((System.Security.Claims.ClaimsIdentity)context.User.Identity).FindFirst(c=> c.Type == "ClientGuid").Value;
            return Guid.Parse(value) is Guid guid ? guid : default(Guid?);
        }
        public static void SetClientGuid(this Microsoft.AspNetCore.Http.HttpContext context, Guid clientGuid)
        {
            ((System.Security.Claims.ClaimsIdentity)context.User.Identity).AddClaim(new System.Security.Claims.Claim("ClientGuid", clientGuid.ToString()));
        }
    }
}
