using FederatedIPAPI.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FederatedIPAPI.Attributes;
using FederatedIPAPI.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FederatedIPAPI.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private IJwtSettings JwtSettings { get; set; }

        public AuthenticationController(IOptions<JwtSettings> jwtSettings)
        {
            JwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        [BasicAuth]
        public IActionResult Post()
        {
            if (HttpContext.GetClientGuid() is System.Guid clientGuid)
            {
                string token = JwtSettings.CreateJwtToken(clientGuid);
                return Ok(token);
            }
            return Unauthorized("User Not Found");
        }
    }
}
