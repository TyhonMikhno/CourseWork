using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using Repositories.Interfaces;

namespace Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly AppDbContext _db;

    public AlbumRepository(AppDbContext db) => _db = db;

    public async Task<List<Album>> GetAllAsync()
        => await _db.Albums.Include(a => a.Artist)
                           .Include(a => a.Tracks)
                           .ToListAsync();

    public async Task<Album?> GetByIdAsync(int id)
        => await _db.Albums.Include(a => a.Artist)
                           .Include(a => a.Tracks)
                           .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Album> CreateAsync(Album album)
    {
        _db.Albums.Add(album);
        await _db.SaveChangesAsync();
        return album;
    }

    public async Task UpdateAsync(Album album)
    {
        var existing = await _db.Albums.FindAsync(album.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException("Album not found");
        }
        else
        { 
            _db.Albums.Update(album);
            await _db.SaveChangesAsync();
        }    
    }

    public async Task DeleteAsync(int id)
    {
        var album = await _db.Albums.FindAsync(id);
        if (album != null)
        {
            _db.Albums.Remove(album);
            await _db.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException("Album not found");
        }
    }
}
