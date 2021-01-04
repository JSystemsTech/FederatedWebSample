using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FederatedIPAPI.TokenProvider;
using FederatedIPAPI.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FederatedIPAPI.Controllers
{
    [Authorize]
    [Route("api/Claims")]
    [ApiController]
    [Produces("application/json")]
    public class ClaimsController : ControllerBase
    {
        private ITokenProvider TokenProvider { get; set; }

        public ClaimsController(ITokenProvider tokenProvider)
        {
            TokenProvider = tokenProvider;
        }
        // POST: api/Claims
        [HttpPost]
        public IEnumerable<TokenClaim> Get([FromBody] TokenParameters model) => TokenProvider.GetClaims(model.Token);

        // POST: api/Claims/{name}
        [HttpPost("{name}")]
        public IEnumerable<string> GetClaim([FromBody] TokenParameters model, string name)
        => TokenProvider.GetClaims(model.Token).FirstOrDefault(c => c.Name == name) is TokenClaim claim ? claim.GetValues(): new string[0];
        

    }
}
