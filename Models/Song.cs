using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using simpleApi.Dto;

namespace simpleApi.Models;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; }

    public ICollection<AlbumSong> AlbumSongs { get; set; }
}