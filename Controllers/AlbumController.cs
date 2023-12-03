using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using simpleApi.Basic;
using simpleApi.Connection;
using simpleApi.Dto;
using simpleApi.Helpers;
using simpleApi.Interface;
using simpleApi.Models;
using simpleApi.Request;
using simpleApi.Service;

namespace simpleApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlbumController : BasicController
{
    private readonly IAlbumService albumService;
    private readonly IExternalDataService externalDataService;
    private readonly DatabaseUtama dbContext;
    private readonly KafkaProducerService _kafkaProducerService;

    public AlbumController(BasicLogger customLogger, BasicConfiguration basicConfiguration, IMapper mapper,
        IAlbumService albumService, DatabaseUtama dbContext, KafkaProducerService kafkaProducerService,
        IExternalDataService externalDataService)
        : base(customLogger, basicConfiguration, mapper)
    {
        _source = GetType().Name;
        this.albumService = albumService;
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.externalDataService = externalDataService;
        _kafkaProducerService = kafkaProducerService;
    }


    [HttpGet("all/")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var albums = dbContext.Albums
                .Include(a => a.Artist)
                .Include(a => a.AlbumSongs)
                .ThenInclude(asong => asong.Song)
                .ToList();

            var albumDTOs = _mapper.Map<List<AlbumDTO>>(albums);
            return StatusCode(StatusCodes.Status202Accepted, albumDTOs);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }

    [HttpGet("api/json")]
    public async Task<IActionResult> GetDummy()
    {
        try
        {
            // var resp = await externalDataService.GetDataPlaceholder();
            var resp = await externalDataService.GetDataPlaceholder();
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.GeneralMessage, resp);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }

    [HttpPost("api")]
    public async Task<IActionResult> ExamplePost(SimplePostRequest request)
    {
        try
        {
            var resp = await externalDataService.PostDataPlaceholder(request);
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.GeneralMessage, resp);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }

    [HttpGet("logger")]
    public async Task<IActionResult> GetLogger()
    {
        try
        {
            var resp = new
            {
                name = "okelah",
                tujuan = "ini hanya tes object logger"
            };

            _customLogger.Log("Information", "Information", _source, "tes message", resp);
            _customLogger.Log("Debug", "Debug", _source, "tes message", resp);
            _customLogger.Log("Trace", "Trace", _source, "tes message", resp);
            _customLogger.Log("Warning", "Warning", _source, "tes message", resp);
            _customLogger.Log("Error", "Error", _source, "tes message", resp);
            _customLogger.Log("Critical", "Critical", _source, "tes message", resp);
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.GeneralMessage);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }

    [HttpPost("kafka/message")]
    public async Task<IActionResult> KafkaMessage([FromBody] SimplePostRequest request)
    {
        try
        {
            var jsonMessage = JsonConvert.SerializeObject(request);
            var messageByte = Helper.StringToByte(jsonMessage);
            var kafkaMessage = new KafkaMessage();
            kafkaMessage.EventTask = "AlbumMessage";
            kafkaMessage.EventData = messageByte;

            await _kafkaProducerService.ProduceMessageAsync(kafkaMessage);
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.GeneralMessage,
                kafkaMessage);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }

    [HttpGet("variable")]
    public IActionResult GetVariable([FromQuery] string variableName)
    {
        try
        {
            if (string.IsNullOrEmpty(variableName)) return BadRequest("Variable name is required.");

            // Ganti dengan logika untuk mendapatkan nilai variabel berdasarkan nama
            var variableValue = _basicConfiguration.GetVariable(variableName);

            if (variableValue == null) return NotFound($"Variable with name '{variableName}' not found.");
            return SetResponse(StatusCodes.Status200OK, BasicCode.GeneralCode, true, BasicMessage.GeneralMessage,
                variableValue);
        }
        catch (Exception e)
        {
            return SetErrorResponse(e);
        }
    }
}