namespace simpleApi.Models;

public class AlbumArtistSongDTO
{
    public int AlbumId { get; set; }
    public string AlbumTitle { get; set; }

    public int ArtistId { get; set; }
    public string ArtistName { get; set; }

    public int SongId { get; set; }
    public string SongTitle { get; set; }
}