using FederatedAuthNAuthZ.Services;
using Newtonsoft.Json;
using ServiceProviderShared;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FederatedAuthNAuthZ.Web
{
    public class EncryptedPostBody
    {
        public string Content { get; set; }
    }
    public abstract class ApiControllerBase : ApiController
    {
        protected HttpResponseMessage TextResponse(string value)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(value);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
        protected HttpResponseMessage EncryptedResponse<T>(T model)
        {
            string value = model is string strModel ? strModel : JsonConvert.SerializeObject(model);
            return TextResponse(value.Encrypt());
        }
        protected string DecryptBody(string bodyValue) => bodyValue.Decrypt();
    }
}
