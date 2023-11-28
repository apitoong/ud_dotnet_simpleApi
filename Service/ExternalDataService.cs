using System.Text;
using Newtonsoft.Json;
using simpleApi.Dto;
using simpleApi.Interface;
using simpleApi.Request;

namespace simpleApi.Service;

public class ExternalDataService : IExternalDataService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalDataService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private async Task<T> SendHttpRequestAsync<T>(HttpMethod method, string path, object requestBody = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://jsonplaceholder.typicode.com" + path;
            var request = new HttpRequestMessage(method, url);

            if (requestBody != null)
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine("pppppppp " + response);
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        catch (HttpRequestException ex)
        {
            // Error HTTP dong
            throw new HttpRequestException($"Terjadi kesalahan saat akses API eksternal: {ex.Message}");
        }
        catch (Exception ex)
        {
            // global error nih
            throw new Exception($"Terjadi kesalahan global: {ex.Message}");
        }
    }


    public async Task<List<ExternalDTO>> GetDataPlaceholder()
    {
        var resp = await SendHttpRequestAsync<List<ExternalDTO>>(HttpMethod.Get, "/posts", null);
        return resp;
    }


    public async Task<SimplePostResponseDTO> PostDataPlaceholder(SimplePostRequest request)
    {
        var resp = await SendHttpRequestAsync<SimplePostResponseDTO>(HttpMethod.Post, "/posts", request);
        return resp;
    }
}