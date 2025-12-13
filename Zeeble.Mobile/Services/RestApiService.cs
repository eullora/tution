using System.Text;

namespace Zeeble.Mobile.Services
{
    public interface IRestApiService
    {
        Task<string> GetAsync(string url);
        Task<string> PostAsync(string endpoint, string body);
        Task<byte[]> GetBytes(string endPoint);
    }

    public class RestApiService : IRestApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "db235653-7446-4827-b241-435ee4e532e5";
        public RestApiService()
        {
            _httpClient = new HttpClient
            { 
                BaseAddress = new Uri("https://api.zeeble.co.in")
            };

            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
        }

        public async Task<string> GetAsync(string endpoint)
        {

            var response = await _httpClient.GetAsync(endpoint);
            var data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return data;
            }

            return null;
        }
        public async Task<string> PostAsync(string endpoint, string body)
        {
            var response = await _httpClient.PostAsync(endpoint, new StringContent(body, Encoding.UTF8, "application/json"));
            var tmp = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
        public async Task<byte[]> GetBytes(string endPoint)
        {
            using var response = await _httpClient.GetAsync(endPoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            return null;
        }
    }
}
