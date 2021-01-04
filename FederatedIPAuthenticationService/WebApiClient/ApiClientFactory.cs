using System.Threading.Tasks;

namespace WebApiClient
{
    public class ApiClientFactory
    {
        public static async Task<IApiClient> CreateAsync(string apiUrl, string authenticationEndpoint, string username, string password)
        => await CreateAsync(apiUrl, authenticationEndpoint, $"{username}:{password}");              
        public static async Task<IApiClient> CreateAsync(string apiUrl, string authenticationEndpoint, string authenticationHeader)
        => await ApiClient.CreateAsync(apiUrl, authenticationEndpoint, authenticationHeader);

        public static IApiClient Create(string apiUrl, string authenticationEndpoint, string username, string password)
        => Create(apiUrl, authenticationEndpoint, $"{username}:{password}");
        public static IApiClient Create(string apiUrl, string authenticationEndpoint, string authenticationHeader)
        => ApiClient.Create(apiUrl, authenticationEndpoint, authenticationHeader);
    }
}
