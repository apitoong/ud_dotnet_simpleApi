using simpleApi.Interface;
using simpleApi.Models;

namespace simpleApi.Service;

public class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _albumRepository;

    public AlbumService(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<Album[]> GetAlbum()
    {
        return await _albumRepository.GetAllAlbumAsync();
    }
}