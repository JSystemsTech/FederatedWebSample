using FederatedIPAPIAuthenticationProviderWeb.Services;
using FederatedIPAPIAuthenticationProviderWeb.TokenProviderApi.Filters;
using FederatedIPAuthenticationService.Services;
using ServiceProvider.ServiceProvider;
using ServiceProvider.Web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace FederatedIPAPIAuthenticationProviderWeb.Controllers.Api
{
    [ApiAuthenticationFilter(false)]
    //[AcceptHeaderJson]
    public abstract class ApiControllerBase : ApiController
    {
        private IServices Services => HttpContext.Current.ApplicationInstance is IMvcServiceApplication app ? app.Services : null;
        protected ITokenProviderService TokenProviderService => Services.Get<ITokenProviderService>();
        protected ITokenProvider TokenProvider => Services.Get<ITokenProvider>();

        protected HttpResponseMessage TextResponse(string value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(value);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
    }
    public class AuthenticationController : ApiControllerBase
    {
        [ApiAuthenticationFilter(true)]
        [HttpPost]
        public HttpResponseMessage Post()
        {
            return TextResponse(TokenProvider.CreateToken(claims=> {
                claims.Add("Name", new string[] { HttpContext.Current.User.Identity.Name });
            }));
        }
    }
    public class RequestController : ApiControllerBase
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody] TokenParameters model) => TextResponse(TokenProviderService.Create(model.GetClaims()));
    }
    public class ClaimsController : ApiControllerBase
    {
        [HttpPost]
        public IEnumerable<TokenClaim> Post([FromBody] TokenParameters model) => TokenProviderService.GetClaims(model.Token);
    }
    public class RenewController : ApiControllerBase
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody] TokenParameters model) => TextResponse(TokenProviderService.Renew(model.Token, model.GetClaims()));
    }
    public class ExpiresController : ApiControllerBase
    {
        [HttpPost]
        public DateTime? Post([FromBody] TokenParameters model) => TokenProviderService.GetExpirationDate(model.Token);
    }
}
