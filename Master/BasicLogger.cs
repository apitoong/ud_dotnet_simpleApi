namespace simpleApi.Basic;

public class BasicLogger
{
    private readonly ILogger<BasicLogger> _logger;

    public BasicLogger(ILogger<BasicLogger> logger)
    {
        _logger = logger;
    }

    public void Log(string level, string type, string source, string message, object data = null)
    {
        switch (level)
        {
            case "Information":
                _logger.LogInformation($"\n{type} -=> {source}\n{message} : {data}\n");
                break;
            case "Debug":
                _logger.LogDebug($"\n{type} -=> {source}\n{message} : {data}\n");
                break;
            case "Trace":
                _logger.LogTrace($"\n{type} -=> {source}\n{message} : {data}\n");
                break;
            case "Warning":
                _logger.LogWarning($"\n{type} -=> {source}\n{message} : {data}\n");
                break;
            case "Error":
                _logger.LogError($"\n{type} -=> {source}\n{message} : {data}\n");
                break;
            case "Critical":
                _logger.LogCritical($"\n{type} -=> {source}\n{message} : {data}\n");
                break;
        }
    }

    public void Debug(string source, string message, object data = null)
    {
        _logger.LogError($"\n{source} -=> {message}\n{data}");
    }
}