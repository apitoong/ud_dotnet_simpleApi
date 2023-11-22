using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Serilog.Context;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString("N");
        var method = context.Request.Method;
        var endpoint = context.Request.Path;
        var backendVersion = "1.0"; // Gantilah sesuai kebutuhan
        var message = "Request Payload";
        var levelName = "DEBUG";
        var channel = "development";
        var datetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        // var requestBody = await FormatRequestBody(context.Request);

        var requestBody = await FormatRequest(context.Request);
        if (!string.IsNullOrEmpty(requestBody)) Console.WriteLine("koosoooooong");

        Console.WriteLine(">>>>>>> 1  " + requestId);
        Console.WriteLine(">>>>>>> 2  " + method);
        Console.WriteLine(">>>>>>> 3  " + endpoint);
        Console.WriteLine(">>>>>>> 4  " + backendVersion);
        Console.WriteLine(">>>>>>> 5  " + message);
        Console.WriteLine(">>>>>>> 6  " + levelName);
        Console.WriteLine(">>>>>>> 7  " + channel);
        Console.WriteLine(">>>>>>> 8  " + datetime);
        Console.WriteLine(">>>>>>> 9  " + ip);
        Console.WriteLine(">>>>>>> 10  " + userAgent);
        Console.WriteLine(requestBody);
        Console.WriteLine(">>>>>>> 11  " + requestBody);

        LogContext.PushProperty("requestId", requestId);
        LogContext.PushProperty("method", method);
        LogContext.PushProperty("endpoint", endpoint);
        LogContext.PushProperty("backend_version", backendVersion);
        LogContext.PushProperty("message", message);
        LogContext.PushProperty("level_name", levelName);
        LogContext.PushProperty("channel", channel);
        LogContext.PushProperty("datetime", datetime);
        LogContext.PushProperty("ip", ip);
        LogContext.PushProperty("user_agent", userAgent);
        LogContext.PushProperty("context", requestBody);

        _logger.LogInformation(message);

        await _next(context);

        if (context.Response != null)
        {
            var responseMessage = "Response Payload";
            var content = context.Response.HttpContext;
            // var content = await ReadAndParseResponseBodyAsync(context.Response.HttpContext.Response.Body);
            var jsonString = await ReadDataFromHttpContextAsync(content.Response.HttpContext);

            Console.WriteLine(content.GetType());
            _logger.LogWarning(jsonString);
            Console.WriteLine("ressss " + jsonString);

            Console.WriteLine("ponnnnn ");
            // var responseBody = await FormatResponse(context.Response);
            // LogContext.PushProperty("message", responseMessage);
            // LogContext.PushProperty("context", responseBody);

            // _logger.LogInformation(responseMessage);
        }
    }


    private async Task<string> ReadDataFromHttpContextAsync(HttpContext context)
    {
        // Menyimpan posisi saat ini
        var originalBody = context.Request.Body;

        try
        {
            // Membaca konten sebagai string secara asinkron
            using (var reader = new StreamReader(originalBody, Encoding.UTF8, true, 1024, true))
            {
                // Pindah ke awal stream
                context.Request.Body.Seek(0, SeekOrigin.Begin);

                return await reader.ReadToEndAsync();
            }
        }
        finally
        {
            // Mengembalikan stream ke posisi awal
            context.Request.Body = originalBody;
        }
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var body = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private async Task<string> FormatStream(HttpResponse response)
    {
        // response.HttpContext.Response;
        var body = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Position = 0;
        return body;
    }


    // private async Task<string> FormatResponse(HttpResponse response)
    // {
    //     var originalBody = response.Body;
    //     using (var memStream = new MemoryStream())
    //     {
    //         response.Body = memStream;
    //
    //         await _next.Invoke(response.HttpContext);
    //
    //         memStream.Position = 0;
    //         var responseBody = new StreamReader(memStream).ReadToEnd();
    //         memStream.Position = 0;
    //         await memStream.CopyToAsync(originalBody);
    //
    //         response.Body = originalBody;
    //         return responseBody;
    //     }
    // }

    private async Task<string> FormatRequestBody(HttpRequest request)
    {
        if (request.Body.CanSeek)
            request.Body.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            try
            {
                return JsonConvert.DeserializeObject(body)?.ToString() ?? body;
            }
            catch
            {
                return body;
            }
        }
    }

    private async Task<string> FormatResponseBody(HttpResponse response)
    {
        var originalBodyStream = response.Body;

        using (var memoryStream = new MemoryStream())
        {
            response.Body = memoryStream;

            await _next(response.HttpContext);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);

            response.Body = originalBodyStream;

            try
            {
                return JsonConvert.DeserializeObject(responseBody)?.ToString() ?? responseBody;
            }
            catch
            {
                return responseBody;
            }
        }
    }
}


// private async Task<string> ReadDataFromHttpContextAsync(HttpContext context)
//     {
//         // Menyimpan posisi saat ini
//         var originalBody = context.Request.Body;
//
//         try
//         {
//             // Membaca konten sebagai string secara asinkron
//             using (var reader = new StreamReader(originalBody, Encoding.UTF8, true, 1024, true))
//             {
//                 // Pindah ke awal stream
//                 context.Request.Body.Seek(0, SeekOrigin.Begin);
//
//                 return await reader.ReadToEndAsync();
//             }
//         }
//         finally
//         {
//             // Mengembalikan stream ke posisi awal
//             context.Request.Body = originalBody;
//         }
//     }