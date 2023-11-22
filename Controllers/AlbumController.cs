using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using simpleApi.Connection;
using simpleApi.Dto;
using simpleApi.Interface;
using simpleApi.Request;

namespace simpleApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlbumController : Controller
{
    private readonly IAlbumService albumService;
    private readonly IExternalDataService externalDataService;
    private readonly DatabaseUtama dbContext;
    private readonly IMapper mapper;
    private readonly ILogger<AlbumController> logger;

    public AlbumController(IAlbumService albumService, DatabaseUtama dbContext, IMapper mapper,
        IExternalDataService externalDataService, ILogger<AlbumController> logger)
    {
        this.albumService = albumService;
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.mapper = mapper;
        this.externalDataService = externalDataService;
        this.logger = logger;
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

            var albumDTOs = mapper.Map<List<AlbumDTO>>(albums);
            return StatusCode(StatusCodes.Status202Accepted, albumDTOs);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("api/")]
    public async Task<IActionResult> GetDummy()
    {
        try
        {
            var resp = await externalDataService.GetDataPlaceholder();

            var result = new
            {
                code = "00",
                status = true,
                message = "success",
                data = resp
            };
            var json = JsonConvert.SerializeObject(result);
            return StatusCode(StatusCodes.Status200OK, json);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("api")]
    public async Task<IActionResult> ExamplePost(SimplePostRequest request)
    {
        try
        {
            var resp = await externalDataService.PostDataPlaceholder(request);

            var result = new
            {
                code = "00",
                status = true,
                message = "success",
                data = resp
            };
            var json = JsonConvert.SerializeObject(result);
            return StatusCode(StatusCodes.Status201Created, json);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
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
            logger.LogInformation($"get all walk request : {resp}");
            logger.LogDebug($"sample logger ->  debug : {resp}");
            logger.LogTrace($"sample logger  -> trace  : {resp}");
            logger.LogWarning($"sample logger  -> warning  : {resp}");
            logger.LogError($"sample logger  -> error  : {resp}");
            logger.LogCritical($"sample logger  -> critical  : {resp}");
            return StatusCode(StatusCodes.Status201Created, resp);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}