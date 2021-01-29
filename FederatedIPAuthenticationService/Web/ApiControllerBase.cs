﻿using FederatedAuthNAuthZ.Services;
using Newtonsoft.Json;
using ServiceProviderShared;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FederatedIPAuthenticationService.Web
{
    public class EncryptedPostBody
    {
        public string Content { get; set; }
    }
    public abstract class ApiControllerBase : ApiController
    {
        protected IEncryptionService EncryptionService => ServiceManager.GetService<IEncryptionService>();
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
        protected string DecryptBody(string bodyValue) => EncryptionService.DateSaltDecrypt(bodyValue, true);
    }
}
