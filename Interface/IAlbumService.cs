using simpleApi.Models;

namespace simpleApi.Interface;

public interface IAlbumService
{
    Task<Album[]> GetAlbum();
}