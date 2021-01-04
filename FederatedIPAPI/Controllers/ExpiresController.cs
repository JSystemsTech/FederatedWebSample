using System;
using Microsoft.AspNetCore.Mvc;
using FederatedIPAPI.TokenProvider;
using FederatedIPAPI.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FederatedIPAPI.Controllers
{
    [Authorize]
    [Route("api/Expires")]
    [ApiController]
    public class ExpiresController : ControllerBase
    {
        private ITokenProvider TokenProvider { get; set; }

        public ExpiresController(ITokenProvider tokenProvider)
        {
            TokenProvider = tokenProvider;
        }

        // POST: api/Expires
        [HttpPost]
        public DateTime? GetTokenDateTime([FromBody] TokenParameters model)
        {
            var ex = TokenProvider.GetExpirationDate(model.Token);
            return ex;
        }

        

    }
}
