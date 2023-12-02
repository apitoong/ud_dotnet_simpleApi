using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using simpleApi.Basic;
using simpleApi.Connection;
using simpleApi.Dto;
using simpleApi.Interface;
using simpleApi.Request;

namespace simpleApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlbumController : BasicController
{
    private readonly IAlbumService albumService;
    private readonly IExternalDataService externalDataService;
    private readonly DatabaseUtama dbContext;


    public AlbumController(IAlbumService albumService, DatabaseUtama dbContext, IMapper mapper,
        IExternalDataService externalDataService, ILogger<AlbumController> logger)
        : base(logger, mapper)
    {
        source = "AlbumController";
        this.albumService = albumService;
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.externalDataService = externalDataService;
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
            return StatusCode(StatusCodes.Status201Created, result);
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

            CostomLogger("Information", "Response", source, "tes message", resp);
            CostomLogger("Information", "Debug", source, "tes message", resp);
            CostomLogger("Information", "Trace", source, "tes message", resp);
            CostomLogger("Information", "Warning", source, "tes message", resp);
            CostomLogger("Information", "Error", source, "tes message", resp);
            CostomLogger("Information", "Critical", source, "tes message", resp);
            return StatusCode(StatusCodes.Status201Created, resp);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}