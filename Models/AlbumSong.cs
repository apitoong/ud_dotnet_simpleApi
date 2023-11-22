namespace simpleApi.Models;

// AlbumSong.cs (Many-to-Many Relationship)
public class AlbumSong
{
    public int AlbumId { get; set; }
    public Album Album { get; set; }

    public int SongId { get; set; }
    public Song Song { get; set; }
}