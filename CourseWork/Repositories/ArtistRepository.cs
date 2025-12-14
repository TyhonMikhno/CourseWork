using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using Repositories.Interfaces;

namespace Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly AppDbContext _db;

    public ArtistRepository(AppDbContext db) => _db = db;

    public async Task<List<Artist>> GetAllAsync()
        => await _db.Artists.Include(a => a.Albums).ToListAsync();

    public async Task<Artist?> GetByIdAsync(int id)
        => await _db.Artists.Include(a => a.Albums)
                            .ThenInclude(al => al.Tracks)
                            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Artist> CreateAsync(Artist artist)
    {
        _db.Artists.Add(artist);
        await _db.SaveChangesAsync();
        return artist;
    }

    public async Task UpdateAsync(Artist artist)
    {
        var existing = await _db.Artists.FindAsync(artist.Id);
        if (existing == null)
        {
        throw new KeyNotFoundException("Artist not found");
        }
        else
        {
            _db.Artists.Update(artist);
            await _db.SaveChangesAsync();
        }    
    }

    public async Task DeleteAsync(int id)
    {
        var artist = await _db.Artists.FindAsync(id);
        if (artist != null)
        {
            _db.Artists.Remove(artist);
            await _db.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException("Artist not found");
        }
    }
}
