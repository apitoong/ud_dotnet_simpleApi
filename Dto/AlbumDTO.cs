using System.ComponentModel.DataAnnotations.Schema;
using simpleApi.Models;

namespace simpleApi.Dto;

// DTOs
public class AlbumDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public ArtistDTO Artist { get; set; }
    public List<SongDTO> Songs { get; set; }
}