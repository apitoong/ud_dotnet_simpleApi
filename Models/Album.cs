using System.ComponentModel.DataAnnotations.Schema;
using simpleApi.Models;


namespace simpleApi.Models;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; }

    public int ArtistId { get; set; }
    public Artist Artist { get; set; }

    public ICollection<AlbumSong> AlbumSongs { get; set; }
}