using System.Text;
using Newtonsoft.Json;
using simpleApi.Basic;
using simpleApi.Dto;
using simpleApi.Interface;
using simpleApi.Request;

namespace simpleApi.Service;

public class ExternalDataService : IExternalDataService
{
    protected readonly BasicLogger _customLogger;
    private readonly BasicConfiguration _basicConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    protected string _source;

    public ExternalDataService(BasicLogger customLogger, BasicConfiguration basicConfiguration,
        IHttpClientFactory httpClientFactory)
    {
        _customLogger = customLogger;
        _basicConfiguration = basicConfiguration;
        _httpClientFactory = httpClientFactory;
        _source = GetType().Name;
    }

    private async Task<T> SendHttpRequestAsync<T>(HttpMethod method, string path, object requestBody = null)
    {
        try
        {
            var url = _basicConfiguration.GetVariable("JSON_PLACE_HOLDER_URL") + path;
            _customLogger.Log("Information", "Request", _source, url);
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(method, url);

            if (requestBody != null)
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            _customLogger.Log("Information", "Response Status", _source, url,
                response.StatusCode.GetHashCode());
            _customLogger.Log("Information", "Response Service", _source, url, jsonString);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        catch (HttpRequestException ex)
        {
            // Error HTTP dong
            _customLogger.Log("Error", "Http Error", _source,
                _basicConfiguration.GetVariable("JSON_PLACE_HOLDER_URL") + path, ex.Message);
            throw new HttpRequestException($"Terjadi kesalahan saat akses API eksternal: {ex.Message}");
        }
        catch (Exception ex)
        {
            // global error nih
            _customLogger.Log("Error", "Global Error", _source,
                _basicConfiguration.GetVariable("JSON_PLACE_HOLDER_URL") + path, ex.Message);
            throw new Exception($"Terjadi kesalahan global: {ex.Message}");
        }
    }


    public async Task<List<ExternalDTO>> GetDataPlaceholder()
    {
        return await SendHttpRequestAsync<List<ExternalDTO>>(HttpMethod.Get, "/posts", null);
    }


    public async Task<SimplePostResponseDTO> PostDataPlaceholder(SimplePostRequest request)
    {
        return await SendHttpRequestAsync<SimplePostResponseDTO>(HttpMethod.Post, "/posts", request);
    }
}