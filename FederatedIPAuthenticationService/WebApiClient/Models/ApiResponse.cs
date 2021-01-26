using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace WebApiClient.Models
{
    public interface IApiResponse
    {
        string Content { get; }
        string Error { get; }
        HttpStatusCode StatusCode { get; }
        Task<T> DeserializeAsync<T>();
        T Deserialize<T>();
    }
    internal class ApiResponse: IApiResponse
    {
        public string Content { get; private set; }
        internal HttpResponseMessage Response { get; set; }
        public string Error { get; private set; }
        public HttpStatusCode StatusCode { get => Response.StatusCode; }
        internal bool IsUnauthorized { get => StatusCode == HttpStatusCode.Unauthorized; }
        internal bool IsForbidden { get => StatusCode == HttpStatusCode.Forbidden; }

        internal static async Task<ApiResponse> BuildAsync(HttpResponseMessage httpResponse, Func<string, Task<string>> decryptionHandlerAsync = null)
        {
            ApiResponse apiResponse = new ApiResponse()
            {
                Response = httpResponse,
                Content = httpResponse.IsSuccessStatusCode ? await httpResponse.Content.ReadAsStringAsync() : null,
                Error = !httpResponse.IsSuccessStatusCode ? $"{httpResponse.StatusCode}: {httpResponse.ReasonPhrase}" : null
            };
            if(decryptionHandlerAsync != null && !string.IsNullOrWhiteSpace(apiResponse.Content))
            {
                apiResponse.Content = await decryptionHandlerAsync(apiResponse.Content);
            }
            await Task.CompletedTask;
            return apiResponse;
        }

        internal static ApiResponse Build(HttpResponseMessage httpResponse, Func<string, string> decryptionHandler = null)
        {
            ApiResponse apiResponse = new ApiResponse()
            {
                Response = httpResponse,
                Content = httpResponse.IsSuccessStatusCode ? httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult() : null,
                Error = !httpResponse.IsSuccessStatusCode ? $"{httpResponse.StatusCode}: {httpResponse.ReasonPhrase}" : null
            };
            if (decryptionHandler != null && !string.IsNullOrWhiteSpace(apiResponse.Content))
            {
                apiResponse.Content = decryptionHandler(apiResponse.Content);
            }
            return apiResponse;
        }
        public T Deserialize<T>()
        {
            T returnValue;

            if (Content is T variable)
                returnValue = variable;
            else
                try
                {
                    //Handling Nullable types i.e, int?, double?, bool? .. etc
                    if (Nullable.GetUnderlyingType(typeof(T)) != null)
                    {
                        TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                        returnValue = (T)conv.ConvertFrom(Content);
                    }
                    else
                    {
                        returnValue = (T)Convert.ChangeType(Content, typeof(T));
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        returnValue = JsonConvert.DeserializeObject<T>(Content);
                    }
                    catch (Exception)
                    {
                        returnValue = default(T);
                    }
                    
                }

            return returnValue;
        }
        public async Task<T> DeserializeAsync<T>()
        {
            T returnValue;

            if (Content is T variable)
                returnValue = variable;
            else
                try
                {
                    //Handling Nullable types i.e, int?, double?, bool? .. etc
                    if (Nullable.GetUnderlyingType(typeof(T)) != null)
                    {
                        TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                        returnValue = (T)conv.ConvertFrom(Content);
                    }
                    else
                    {
                        returnValue = (T)Convert.ChangeType(Content, typeof(T));
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        returnValue = JsonConvert.DeserializeObject<T>(Content);
                    }
                    catch (Exception)
                    {
                        returnValue = default(T);
                    }

                }

            await Task.CompletedTask;
            return returnValue;
        }
    }
}
