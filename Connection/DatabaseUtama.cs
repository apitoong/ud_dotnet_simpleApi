using Microsoft.EntityFrameworkCore;
using simpleApi.Models;

namespace simpleApi.Connection;

public class DatabaseUtama : DbContext
{
    public DatabaseUtama(DbContextOptions<DatabaseUtama> dbContextOptions)
        : base(dbContextOptions)
    {
    }


    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<AlbumSong> AlbumSong { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Pindahkan ini ke awal metode
        modelBuilder.Entity<AlbumSong>().HasKey(x => new { x.AlbumId, x.SongId });
    }
}