using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using WebApiClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;

namespace WebApiClient
{   
    public interface IApiClient
    {
        Task<Func<T, Task<IApiResponse>>> CreatePostMethodAsync<T>(string endpoint);
        Task<Func<T, Task<IApiResponse>>> CreatePutMethodAsync<T>(string endpoint);
        Task<Func<T, Task<IApiResponse>>> CreatePatchMethodAsync<T>(string endpoint);
        Task<Func<T, Task<IApiResponse>>> CreateGetMethodAsync<T>(string endpoint);
        Task<Func<T, Task<IApiResponse>>> CreateDeleteMethodAsync<T>(string endpoint);

        IApiEndpoint CreateEndpoint(string endpoint);
        Func<T, IApiResponse> CreatePostMethod<T>(string endpoint);
        Func<T, IApiResponse> CreatePutMethod<T>(string endpoint);
        Func<T, IApiResponse> CreatePatchMethod<T>(string endpoint);
        Func<T, IApiResponse> CreateGetMethod<T>(string endpoint);
        Func<T, IApiResponse> CreateDeleteMethod<T>(string endpoint);
    }
    internal class ApiClient : IApiClient
    {
        private static HttpClient Client = new HttpClient();
        internal string ApiUrl { get; private set; }
        private IApiEndpoint AuthenticationEndpoint { get; set; }
        protected AuthenticationHeaderValue InitialAuthenticationHeader { get; set; }
        protected AuthenticationHeaderValue TokenAuthenticationHeader { get; set; }
        private AuthenticationHeaderValue AuthenticationHeader { get => CallingAPIAuthenticaion ? InitialAuthenticationHeader : TokenAuthenticationHeader; }
        private bool CallingAPIAuthenticaion { get; set; }
        internal ApiClient(string apiUrl) {
            ApiUrl = apiUrl;
        }
        private ApiClient(string apiUrl, string authenticationString)
        {
            ApiUrl = apiUrl;
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            InitialAuthenticationHeader = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), base64EncodedAuthenticationString);
        }
        public static ApiClient Create(string apiUrl, string authenticationEndpoint, string authenticationHeader)
        {
            ApiClient apiClient = new ApiClient(apiUrl, authenticationHeader);
            apiClient.AuthenticationEndpoint = apiClient.CreateEndpoint(authenticationEndpoint);
            apiClient.Authenticate();
            return apiClient;
        }
        protected virtual void Authenticate()
        {
            CallingAPIAuthenticaion = true;
            IApiResponse authResponse = AuthenticationEndpoint.Post(string.Empty);
            TokenAuthenticationHeader = new AuthenticationHeaderValue("Bearer", authResponse.Content);
            CallingAPIAuthenticaion = false;
        }
        protected virtual async Task AuthenticateAsync()
        {
            CallingAPIAuthenticaion = true;
            IApiResponse authResponse = await AuthenticationEndpoint.PostAsync(string.Empty);
            TokenAuthenticationHeader = new AuthenticationHeaderValue("Bearer", authResponse.Content);
            CallingAPIAuthenticaion = false;
        }
        public static async Task<IApiClient> CreateAsync(string apiUrl, string authenticationEndpoint, string authenticationHeader)
        {
            ApiClient apiClient = new ApiClient(apiUrl, authenticationHeader);
            apiClient.AuthenticationEndpoint = await apiClient.CreateEndpointAsync(authenticationEndpoint);
            await apiClient.AuthenticateAsync();
            await Task.CompletedTask;
            return apiClient;
        }
        public async Task<IApiEndpoint> CreateEndpointAsync(string endpoint)
        {
            IApiEndpoint apiEndpoint = new ApiEndpoint(this, endpoint);
            await Task.CompletedTask;
            return apiEndpoint;
        }
        public async Task<Func<T, Task<IApiResponse>>> CreatePostMethodAsync<T>(string endpoint) => (await CreateEndpointAsync(endpoint)).PostAsync;
        public async Task<Func<T, Task<IApiResponse>>> CreatePutMethodAsync<T>(string endpoint) => (await CreateEndpointAsync(endpoint)).PutAsync;
        public async Task<Func<T, Task<IApiResponse>>> CreatePatchMethodAsync<T>(string endpoint) => (await CreateEndpointAsync(endpoint)).PatchAsync;
        public async Task<Func<T, Task<IApiResponse>>> CreateGetMethodAsync<T>(string endpoint) => (await CreateEndpointAsync(endpoint)).GetAsync;
        public async Task<Func<T, Task<IApiResponse>>> CreateDeleteMethodAsync<T>(string endpoint) => (await CreateEndpointAsync(endpoint)).DeleteAsync;

        public IApiEndpoint CreateEndpoint(string endpoint) => new ApiEndpoint(this, endpoint);
        public Func<T,IApiResponse> CreatePostMethod<T>(string endpoint) => CreateEndpoint(endpoint).Post;
        public Func<T, IApiResponse> CreatePutMethod<T>(string endpoint) => CreateEndpoint(endpoint).Put;
        public Func<T, IApiResponse> CreatePatchMethod<T>(string endpoint) => CreateEndpoint(endpoint).Patch;
        public Func<T, IApiResponse> CreateGetMethod<T>(string endpoint) => CreateEndpoint(endpoint).Get;
        public Func<T, IApiResponse> CreateDeleteMethod<T>(string endpoint) => CreateEndpoint(endpoint).Delete;


        private async Task<HttpResponseMessage> ExecuteClientMethodAsync(string url, HttpMethod method, string serializedContent = null)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage() { RequestUri = new Uri(url), Method = method };
            try
            {

                //request.Headers.Authorization = AuthenticationHeader;
                var initialRequestURI = requestMessage.RequestUri;
                if (serializedContent != null)
                {
                    requestMessage.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
                }
                requestMessage.Headers.Authorization = AuthenticationHeader;
                Client.DefaultRequestHeaders.Authorization = AuthenticationHeader;

                requestMessage.Headers.Accept.Clear();

                HttpResponseMessage response = await Client.SendAsync(requestMessage);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var finalRequestURI = response.RequestMessage.RequestUri;
                    /*Detect if request redirected to another url*/
                    if (finalRequestURI != initialRequestURI)
                    {
                        using (HttpRequestMessage redirectedRequest = new HttpRequestMessage() { RequestUri = finalRequestURI, Method = method })
                        {
                            if (serializedContent != null)
                            {
                                redirectedRequest.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
                            }
                            redirectedRequest.RequestUri = finalRequestURI;
                            redirectedRequest.Headers.Authorization = AuthenticationHeader;
                            redirectedRequest.Headers.Accept.Clear();
                            response = await Client.SendAsync(redirectedRequest);
                        }

                    }
                }
                return response;

            }
            catch (Exception)
            {
                return new HttpResponseMessage();
            }
        }
        private Func<string, string, Task<HttpResponseMessage>> GetSendAsync(HttpMethod method)
        => async (url, httpContent) =>
        {
            return await ExecuteClientMethodAsync(url, method, httpContent);
        };
        private Func<string, Task<HttpResponseMessage>> GetSendAsyncNoData(HttpMethod method)
        => async (url) =>
        {
            return await ExecuteClientMethodAsync(url, method);
        };

        private HttpResponseMessage ExecuteClientMethod(string url, HttpMethod method, string serializedContent = null)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage() { RequestUri = new Uri(url), Method = method };
            try
            {
                
                    //request.Headers.Authorization = AuthenticationHeader;
                    var initialRequestURI = requestMessage.RequestUri;
                    if(serializedContent != null)
                    {
                        requestMessage.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
                    }
                    requestMessage.Headers.Authorization = AuthenticationHeader;
                    Client.DefaultRequestHeaders.Authorization = AuthenticationHeader;

                    requestMessage.Headers.Accept.Clear();
                    
                    HttpResponseMessage response = Client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    if(response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var finalRequestURI = response.RequestMessage.RequestUri;
                        /*Detect if request redirected to another url*/
                        if(finalRequestURI != initialRequestURI)
                        {
                            using (HttpRequestMessage redirectedRequest = new HttpRequestMessage() { RequestUri = finalRequestURI, Method = method })
                            {
                            if (serializedContent != null)
                            {
                                redirectedRequest.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
                            }
                            redirectedRequest.RequestUri = finalRequestURI;
                                redirectedRequest.Headers.Authorization = AuthenticationHeader;
                                Client.DefaultRequestHeaders.Authorization = AuthenticationHeader;


                                redirectedRequest.Headers.Accept.Clear();
                                
                                response = Client.SendAsync(redirectedRequest).GetAwaiter().GetResult();
                            }
                                
                        }
                    }
                    return response;

            }
            catch (Exception)
            {
                return new HttpResponseMessage();
            }
        }
        private Func<string, string, HttpResponseMessage> GetSend(HttpMethod method)
        => (url, serializedContent) =>
        {
            return ExecuteClientMethod(url, method, serializedContent);
        };
        private Func<string, HttpResponseMessage> GetSendNoData(HttpMethod method)
        => (url) =>
        {
            return ExecuteClientMethod(url, method);
        };


        private static HttpMethod HttpMethodPatch = new HttpMethod("PATCH");
        private KeyValuePair<string, string> BuildQueryParameter(object key, object value)
        => new KeyValuePair<string, string>(key.ToString(), HttpUtility.UrlEncode(value.ToString()));
        private string BuildUrlWithQueryParams<T>(string url, T data)
        {
            List<KeyValuePair<string, string>> queryList = new List<KeyValuePair<string, string>>();
            if (data is IDictionary collection)
            {
                foreach (object key in collection.Keys)
                {
                    object collectionValue = collection[key];
                    if (collectionValue != null)
                    {
                        if (!(collectionValue is string) && collectionValue is IEnumerable values)
                        {
                            foreach (object value in values)
                            {
                                queryList.Add(BuildQueryParameter(key, value));
                            }
                        }
                        else
                        {
                            queryList.Add(BuildQueryParameter(key, collectionValue));
                        }

                    }
                }
            }
            else
            {
                var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(prop => prop.GetValue(data, null) != null);
                foreach (PropertyInfo prop in props)
                {
                    object propValue = prop.GetValue(data, null);
                    if (propValue != null)
                    {
                        if (!(propValue is string) && propValue is IEnumerable values)
                        {
                            foreach (object value in values)
                            {
                                queryList.Add(BuildQueryParameter(prop.Name, value));
                            }
                        }
                        else
                        {
                            queryList.Add(BuildQueryParameter(prop.Name, propValue));
                        }

                    }
                }

            }

            return $"{url}{string.Join("&", queryList.Select(qp => $"{qp.Key}={qp.Value}"))}";
        }

        private async Task<IApiResponse> CallApiAsync<T>(Func<string, string,Task<HttpResponseMessage>> request, string url, T content)
        {
            string serializedContent = JsonConvert.SerializeObject(content);
            
            HttpResponseMessage httpResponse = await request(url, serializedContent);

            ApiResponse response = await ApiResponse.BuildAsync(httpResponse);
            if (response.IsUnauthorized && !CallingAPIAuthenticaion)
            {
                await AuthenticateAsync();
                httpResponse = await request(url, serializedContent);
                response = await ApiResponse.BuildAsync(httpResponse);
            }
            await Task.CompletedTask;
            return response;
        }
        private async Task<IApiResponse> CallApiAsync<T>(Func<string, Task<HttpResponseMessage>> request, string url, T data)
        {

            string requestUrl = BuildUrlWithQueryParams(url, data);
            HttpResponseMessage httpResponse = await request(requestUrl);
            ApiResponse response = await ApiResponse.BuildAsync(httpResponse);
            if (response.IsUnauthorized && !CallingAPIAuthenticaion)
            {
                await AuthenticateAsync();
                httpResponse = await request(requestUrl);
                response = await ApiResponse.BuildAsync(httpResponse);
            }
            await Task.CompletedTask;
            return response;
        }

        internal async Task<IApiResponse> PostAsync<TContent>(string url, TContent data)
        => await SendAsync(url, data, HttpMethod.Post);
        internal async Task<IApiResponse> PutAsync<TContent>(string url, TContent data)
        => await SendAsync(url, data, HttpMethod.Put);
        internal async Task<IApiResponse> GetAsync<TContent>(string url, TContent data)
        => await SendWithQueryParamsAsync(url, data, HttpMethod.Get);
        internal async Task<IApiResponse> DeleteAsync<TContent>(string url, TContent data)
        => await SendWithQueryParamsAsync(url, data, HttpMethod.Delete );
        internal async Task<IApiResponse> PatchAsync<TContent>(string url, TContent data)
            => await SendAsync(url, data, HttpMethodPatch);
        internal async Task<IApiResponse> SendAsync<TContent>(string url, TContent data, HttpMethod httpMethod)
        {
            Func<string, string, Task<HttpResponseMessage>> handler = GetSendAsync(httpMethod);
            return await CallApiAsync(handler, url, data); ;
        }
        internal async Task<IApiResponse> SendWithQueryParamsAsync<TContent>(string url, TContent data, HttpMethod httpMethod)
        {
            Func<string, Task<HttpResponseMessage>> handler = GetSendAsyncNoData(httpMethod);
            return await CallApiAsync(handler, url, data); ;
        }

        private IApiResponse CallApi<T>(Func<string, string, HttpResponseMessage> request, string url, T content)
        {
            string serializedContent = JsonConvert.SerializeObject(content);
            HttpResponseMessage httpResponse = request(url, serializedContent);

            ApiResponse response = ApiResponse.Build(httpResponse);
            if (response.IsUnauthorized && !CallingAPIAuthenticaion)
            {
                Authenticate();
                httpResponse = request(url, serializedContent);
                response = ApiResponse.Build(httpResponse);
            }
            return response;
        }
        
        private IApiResponse CallApi<T>(Func<string, HttpResponseMessage> request, string url, T data)
        {
            string requestUrl = BuildUrlWithQueryParams(url, data);
            HttpResponseMessage httpResponse = request(requestUrl);
            ApiResponse response = ApiResponse.Build(httpResponse);
            if (response.IsUnauthorized && !CallingAPIAuthenticaion)
            {
                Authenticate();
                httpResponse = request(requestUrl);
                response = ApiResponse.Build(httpResponse);
            }
            return response;
        }

        internal IApiResponse Post<TContent>(string url, TContent data)
        => Send(url, data, HttpMethod.Post);
        internal IApiResponse Put<TContent>(string url, TContent data)
        => Send(url, data, HttpMethod.Put);
        internal IApiResponse Get<TContent>(string url, TContent data)
        => SendWithQueryParams(url, data, HttpMethod.Get);
        internal IApiResponse Delete<TContent>(string url, TContent data)
        => SendWithQueryParams(url, data, HttpMethod.Delete);
        internal IApiResponse Patch<TContent>(string url, TContent data)
        => Send(url, data, HttpMethodPatch);
        internal IApiResponse Send<TContent>(string url, TContent data, HttpMethod httpMethod)
        {
            Func<string, string, HttpResponseMessage> handler = GetSend(httpMethod);
            return CallApi(handler, url, data); ;
        }
        internal IApiResponse SendWithQueryParams<TContent>(string url, TContent data, HttpMethod httpMethod)
        {
            Func<string, HttpResponseMessage> handler = GetSendNoData(httpMethod);
            return CallApi(handler, url, data); ;
        }
    }
}
