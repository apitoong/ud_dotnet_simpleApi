using System.Dynamic;
using System.Text;
using Newtonsoft.Json;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        await LogRequest(context);

        var originalResponseBody = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;
            await _next.Invoke(context);
            await LogResponse(context, responseBody, originalResponseBody);

            // Dispose MemoryStream
            responseBody.Dispose();
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        var requestContent = new StringBuilder();

        requestContent.AppendLine("\n=== Request Information Start ===");
        IDictionary<string, object> headerData = new Dictionary<string, object> { };
        foreach (var (headerKey, headerValue) in context.Request.Headers)
            headerData.Add(headerKey, headerValue[0]);

        context.Request.EnableBuffering();
        var requestReader = new StreamReader(context.Request.Body, Encoding.UTF8);
        var content = await requestReader.ReadToEndAsync();

        var requestId = Guid.NewGuid().ToString();
        context.Items["request_id"] = requestId;
        dynamic requestData = new ExpandoObject();
        requestData.request_id = requestId;
        requestData.method = context.Request.Method.ToUpper();
        requestData.url = context.Request.Path;
        requestData.ip = context.Connection.RemoteIpAddress?.ToString();
        requestData.message = "Request Payload";
        requestData.level_name = "DEBUG";
        requestData.channel = "development";
        requestData.date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        requestData.timezone = TimeZoneInfo.Local.Id;
        requestData.headers = headerData;
        requestData.context = JsonConvert.DeserializeObject(content);

        string jsonRequestData = JsonConvert.SerializeObject(requestData);
        requestContent.AppendLine($"{jsonRequestData}");
        requestContent.AppendLine("=== Request Information End ===");
        _logger.LogInformation(requestContent.ToString());
        context.Request.Body.Position = 0;
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalResponseBody)
    {
        var responseContent = new StringBuilder();
        responseContent.AppendLine("\n=== Response Information Start ===");

        IDictionary<string, object> headerData = new Dictionary<string, object> { };
        foreach (var (headerKey, headerValue) in context.Request.Headers)
            headerData.Add(headerKey, headerValue[0]);

        responseBody.Position = 0;
        var content = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalResponseBody);
        context.Response.Body = originalResponseBody;

        dynamic responseData = new ExpandoObject();
        responseData.request_id = context.Items["request_id"]?.ToString();
        responseData.method = context.Request.Method.ToUpper();
        responseData.url = context.Request.Path;
        responseData.ip = context.Connection.RemoteIpAddress?.ToString();
        responseData.message = "Response Payload";
        responseData.level_name = "DEBUG";
        responseData.channel = "development";
        responseData.date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        responseData.timezone = TimeZoneInfo.Local.Id;
        responseData.headers = headerData;
        responseData.context = JsonConvert.DeserializeObject(content);

        string jsonResponseData = JsonConvert.SerializeObject(responseData);
        responseContent.AppendLine($"{jsonResponseData}");

        responseContent.AppendLine("=== Response Information End ===");
        _logger.LogInformation(responseContent.ToString());
    }
}