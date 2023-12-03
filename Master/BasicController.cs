using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using simpleApi.Helpers;
using simpleApi.Models;

namespace simpleApi.Basic;

public class BasicController : Controller
{
    protected readonly BasicLogger _basicLogger;
    protected readonly BasicConfiguration _basicConfiguration;
    protected readonly IMapper _mapper;
    protected string _source;
    private GlobalResponse _response;


    public BasicController(BasicLogger basicLogger, BasicConfiguration basicConfiguration, IMapper mapper)
    {
        _basicLogger = basicLogger;
        _basicConfiguration = basicConfiguration;
        _mapper = mapper;
        _source = GetType().Name;
        _response = new GlobalResponse();
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    public ObjectResult SetResponse(int statusCode, string code, bool status, string message,
        object data = null)
    {
        _response.Code = code;
        _response.Status = status;
        _response.Title = status ? "Success" : "Error";
        _response.Message = message;
        _response.Data = data;

        return StatusCode(statusCode, _response);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public ObjectResult SetErrorResponse(Exception e)
    {
        _response.Code = BasicCode.GeneralErrorCode;
        _response.Status = false;
        _response.Title = "Error";
        _response.Message = BasicMessage.GeneralErrorMessage;
        _response.Data = null;

        var enableDebugMessage = Helper.StringToBoolean(_basicConfiguration.GetVariable("ENABLE_DEBUG_MESSAGE"));

        if (enableDebugMessage) _response.Message = e.Message;

        return StatusCode(StatusCodes.Status500InternalServerError, _response);
    }
}