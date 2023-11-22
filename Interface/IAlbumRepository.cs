using simpleApi.Models;

namespace simpleApi.Interface;

public interface IAlbumRepository
{
    Task<Album[]> GetAllAlbumAsync();
}