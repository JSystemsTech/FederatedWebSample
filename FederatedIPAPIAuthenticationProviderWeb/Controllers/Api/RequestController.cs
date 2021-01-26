using FederatedIPAPIAuthenticationProviderWeb.Services;
using FederatedIPAPIAuthenticationProviderWeb.TokenProviderApi.Filters;
using FederatedIPAuthenticationService.Services;
using Newtonsoft.Json;
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

        private IEncryptionService EncryptionService => Services.Get<IEncryptionService>();
        protected HttpResponseMessage TextResponse(string value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(value);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
        protected HttpResponseMessage EncryptedResponse<T>(T model)
        {
            string value = EncryptionService.DateSaltEncrypt(model is string strModel ? strModel : JsonConvert.SerializeObject(model));
            return TextResponse(value);
        }
        protected string DecryptBody(EncryptedPostBody body) => EncryptionService.DateSaltDecrypt(body.Content, true);
        protected TokenParameters GetBodyAsTokenParameters(EncryptedPostBody body)=> JsonConvert.DeserializeObject<TokenParameters>(DecryptBody(body));
    }
    public class EncryptedPostBody
    {
        public string Content { get; set; }
    }
    public class AuthenticationController : ApiControllerBase
    {
        [ApiAuthenticationFilter(true)]
        [HttpPost]
        public HttpResponseMessage Post()
        {
            return EncryptedResponse(TokenProvider.CreateToken(claims=> {
                claims.Add("Name", new string[] { HttpContext.Current.User.Identity.Name });
            }));
        }
    }
    public class RequestController : ApiControllerBase
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody] EncryptedPostBody body) => EncryptedResponse(TokenProviderService.Create(GetBodyAsTokenParameters(body).GetClaims()));
    }
    public class ClaimsController : ApiControllerBase
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody] EncryptedPostBody body) => EncryptedResponse(TokenProviderService.GetClaims(GetBodyAsTokenParameters(body).Token));
    }
    public class RenewController : ApiControllerBase
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody] EncryptedPostBody body) {
            TokenParameters model = GetBodyAsTokenParameters(body);
            return EncryptedResponse(TokenProviderService.Renew(model.Token, model.GetClaims())); 
        }
    }
    public class ExpiresController : ApiControllerBase
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody] EncryptedPostBody body) => EncryptedResponse(TokenProviderService.GetExpirationDate(GetBodyAsTokenParameters(body).Token));
    }
}
