using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using simpleApi.Models;

namespace simpleApi.Basic;

public class BasicController : Controller
{
    protected readonly ILogger<BasicController> logger;
    protected readonly IMapper mapper;
    protected string source;
    private GlobalResponse response;


    public BasicController(ILogger<BasicController> logger, IMapper mapper)
    {
        this.mapper = mapper;
        this.logger = logger;
        response = new GlobalResponse();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void CostomLogger(string level, string type, string source, string message, object data)
    {
        switch (level)
        {
            case "Information":
                logger.LogInformation($"\n{type} -> {source}\n{message} : {data}\n");
                break;
            case "Debug":
                logger.LogDebug($"\n{type} -> {source}\n{message} : {data}\n");
                break;
            case "Trace":
                logger.LogTrace($"\n{type} -> {source}\n{message} : {data}\n");
                break;
            case "Warning":
                logger.LogWarning($"\n{type} -> {source}\n{message} : {data}\n");
                break;
            case "Error":
                logger.LogError($"\n{type} -> {source}\n{message} : {data}\n");
                break;
            case "Critical":
                logger.LogCritical($"\n{type} -> {source}\n{message} : {data}\n");
                break;
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public ObjectResult SetResponse(int statusCode, string code, bool status, string message,
        object data = null)
    {
        response.Code = code;
        response.Status = status;
        response.Title = status ? "Success" : "Error";
        response.Message = message;
        response.Data = data;

        return StatusCode(statusCode, response);
    }
}