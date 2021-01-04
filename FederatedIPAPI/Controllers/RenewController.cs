using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FederatedIPAPI.TokenProvider;
using FederatedIPAPI.Models;
using Microsoft.AspNetCore.Authorization;
using FederatedIPAPI.Attributes;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FederatedIPAPI.Controllers
{
    //[ServiceFilter(typeof(MyTrackingActionFilter))]
    [Authorize]
    [Route("api/Renew")]
    [ApiController]
    public class RenewController : ControllerBase
    {
        private ITokenProvider TokenProvider { get; set; }

        public RenewController(ITokenProvider tokenProvider)
        {
            TokenProvider = tokenProvider;
        }
        // POST: api/Token
        [HttpPost]
        public string RenewToken([FromBody] TokenParameters model) => TokenProvider.Renew(model.Token, model.GetClaims());

        // POST: api/Token/{name}
        [HttpPost("{name}")]
        public string RenewTokenWithOneClaim([FromBody] TokenParameters model, string name)
            => TokenProvider.Renew(model.Token, new TokenClaim(name, model.GetValues()));

    }
}
