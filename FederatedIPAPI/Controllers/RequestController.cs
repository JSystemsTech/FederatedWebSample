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
    [Route("api/Request")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private ITokenProvider TokenProvider { get; set; }

        public RequestController(ITokenProvider tokenProvider)
        {
            TokenProvider = tokenProvider;
        }
        // GET: api/Request
        [HttpPost]
        public string RequestToken([FromBody] TokenParameters model) => TokenProvider.Create(model.GetClaims());

        // GET: api/Request/{name}
        [HttpPost("{name}")]
        public string RequestTokenWithOneClaim([FromBody] TokenParameters model, string name) => TokenProvider.Create(new TokenClaim[1] { new TokenClaim(name, model.GetValues()) });


    }
}
