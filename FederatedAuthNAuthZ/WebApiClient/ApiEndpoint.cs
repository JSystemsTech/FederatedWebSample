using WebApiClient.Models;
using System.Threading.Tasks;
namespace WebApiClient
{
    public interface IApiEndpoint
    {
        Task<IApiResponse> PostAsync<TContent>(TContent data);
        Task<IApiResponse> PutAsync<TContent>(TContent data);
        Task<IApiResponse> GetAsync<TContent>(TContent data);
        Task<IApiResponse> DeleteAsync<TContent>(TContent data);
        Task<IApiResponse> PatchAsync<TContent>(TContent data);

        IApiResponse Post<TContent>(TContent data);
        IApiResponse Put<TContent>(TContent data);
        IApiResponse Get<TContent>(TContent data);
        IApiResponse Delete<TContent>(TContent data);
        IApiResponse Patch<TContent>(TContent data);
    }
    internal class ApiEndpoint: IApiEndpoint
    {
        private ApiClient ApiClient { get; set; }
        private string EndpointUrl { get; set; }

        public ApiEndpoint(ApiClient apiClient, string endpoint)
        {
            ApiClient = apiClient;
            EndpointUrl = $"{ApiClient.ApiUrl}/{endpoint}?";
        }
        public async Task<IApiResponse> PostAsync<TContent>(TContent data)
        => await ApiClient.PostAsync(EndpointUrl, data);
        public async Task<IApiResponse> PutAsync<TContent>(TContent data)
        => await ApiClient.PutAsync(EndpointUrl, data);
        public async Task<IApiResponse> GetAsync<TContent>(TContent data)
        => await ApiClient.GetAsync(EndpointUrl, data);
        public async Task<IApiResponse> DeleteAsync<TContent>(TContent data)
        => await ApiClient.DeleteAsync(EndpointUrl, data);
        public async Task<IApiResponse> PatchAsync<TContent>(TContent data)
        => await ApiClient.PatchAsync(EndpointUrl, data);


        public IApiResponse Post<TContent>(TContent data)
        =>  ApiClient.Post(EndpointUrl, data);
        public IApiResponse Put<TContent>(TContent data)
        =>  ApiClient.Put(EndpointUrl, data);
        public IApiResponse Get<TContent>(TContent data)
        =>  ApiClient.Get(EndpointUrl, data);
        public IApiResponse Delete<TContent>(TContent data)
        =>  ApiClient.Delete(EndpointUrl, data);
        public IApiResponse Patch<TContent>(TContent data)
        =>  ApiClient.Patch(EndpointUrl, data);
    }
}
