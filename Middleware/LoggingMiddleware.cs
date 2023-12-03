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
        context.Request.EnableBuffering();
        var requestReader = new StreamReader(context.Request.Body, Encoding.UTF8);
        var content = await requestReader.ReadToEndAsync();
        createLogger("Request", context, content);
        context.Request.Body.Position = 0;
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalResponseBody)
    {
        responseBody.Position = 0;
        var content = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalResponseBody);
        context.Response.Body = originalResponseBody;
        createLogger("Response", context, content);
    }

    private IDictionary<string, object> getHeaderContent(IHeaderDictionary header)
    {
        IDictionary<string, object> headerData = new Dictionary<string, object> { };
        foreach (var (headerKey, headerValue) in header)
            headerData.Add(headerKey, headerValue[0]);

        return headerData;
    }

    private ExpandoObject getDetailAPi(HttpContext context, string message)
    {
        dynamic result = new ExpandoObject();
        context.Items["request_id"] = context.Items["request_id"] is null
            ? Guid.NewGuid().ToString()
            : context.Items["request_id"]?.ToString();
        result.request_id = context.Items["request_id"]?.ToString();
        result.method = context.Request.Method.ToUpper();
        result.url = context.Request.Path;
        result.ip = context.Connection.RemoteIpAddress?.ToString();
        result.message = $"{message} Payload";
        result.level_name = "DEBUG";
        result.channel = "development";
        result.date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        result.timezone = TimeZoneInfo.Local.Id;

        return result;
    }

    private void createLogger(string message, HttpContext context, string contentBody)
    {
        var headerContent = getHeaderContent(context.Request.Headers);
        var contentDetail = getDetailAPi(context, message);

        var loggerContent = new StringBuilder();
        loggerContent.AppendLine($"\n=== {message} Information Start ===");
        loggerContent.AppendLine($"\n{message} Detail -=> {JsonConvert.SerializeObject(contentDetail)}");
        loggerContent.AppendLine($"\n{message} Header -=> {JsonConvert.SerializeObject(headerContent)}");
        loggerContent.AppendLine(
            $"\n{message} Body -=> {JsonConvert.SerializeObject(JsonConvert.DeserializeObject(contentBody))}");
        loggerContent.AppendLine($"\n=== {message} Information End ===");

        _logger.LogInformation(loggerContent.ToString());
    }
}