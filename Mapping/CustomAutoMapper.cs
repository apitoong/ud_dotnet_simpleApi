using AutoMapper;
using simpleApi.Dto;
using simpleApi.Models;


namespace simpleApi.Mapping;

public class CustomAutoMapper : Profile
{
    public CustomAutoMapper()
    {
        // CreateMap<Album, AlbumDto>()
        //     .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Artists))
        //     .ForMember(dest => dest.Songs, opt => opt.MapFrom(src => src.Songs))
        //     .ReverseMap();
        //
        // CreateMap<Artist, ArtistDto>()
        //     .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => src.Albums))
        //     .ForMember(dest => dest.Songs, opt => opt.MapFrom(src => src.Songs))
        //     .ReverseMap();
        //
        // CreateMap<Song, SongDto>()
        //     .ForMember(dest => dest.Artist, opt => opt.MapFrom(src => src.Artist))
        //     .ForMember(dest => dest.Album, opt => opt.MapFrom(src => src.Album))
        //     .ReverseMap();

        CreateMap<Album, AlbumDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Artist, opt => opt.MapFrom(src => src.Artist))
            .ForMember(dest => dest.Songs,
                opt => opt.MapFrom(src => src.AlbumSongs.Select(asong => asong.Song).ToList()));

        CreateMap<Artist, ArtistDTO>();
        CreateMap<Song, SongDTO>();
        CreateMap<Album, AlbumArtistSongDTO>();
    }
}