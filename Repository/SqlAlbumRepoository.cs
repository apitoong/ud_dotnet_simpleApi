using AutoMapper;
using Microsoft.EntityFrameworkCore;
using simpleApi.Connection;
using simpleApi.Dto;
using simpleApi.Models;
using simpleApi.Interface;

namespace simpleApi.Repository;

public class SqlAlbumRepoository : IAlbumRepository
{
    private readonly DatabaseUtama dbContext;
    private readonly IMapper mapper;

    public SqlAlbumRepoository(DatabaseUtama dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Album[]> GetAllAlbumAsync()
    {
        return await dbContext.Albums
            .Include(a => a.Artist)
            .Include(a => a.AlbumSongs)
            .ThenInclude(asong => asong.Song)
            .ToArrayAsync();
    }
}