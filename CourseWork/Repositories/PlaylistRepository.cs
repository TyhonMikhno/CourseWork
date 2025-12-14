using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using Repositories.Interfaces;

namespace Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly AppDbContext _db;

    public PlaylistRepository(AppDbContext db) => _db = db;

    public async Task<List<Playlist>> GetAllAsync()
        => await _db.Playlists.Include(p => p.PlaylistTracks)
                               .ThenInclude(pt => pt.Track)
                               .ToListAsync();

    public async Task<Playlist?> GetByIdAsync(int id)
        => await _db.Playlists.Include(p => p.PlaylistTracks)
                               .ThenInclude(pt => pt.Track)
                               .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Playlist> CreateAsync(Playlist playlist)
    {
        _db.Playlists.Add(playlist);
        await _db.SaveChangesAsync();
        return playlist;
    }

    public async Task UpdateAsync(Playlist playlist)
    {
        var existing = await _db.Playlists.FindAsync(playlist.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException("Playlist not found");
        }
        else
        {
            _db.Playlists.Update(playlist);
            await _db.SaveChangesAsync();
        }    
    }

    public async Task DeleteAsync(int id)
    {
        var playlist = await _db.Playlists.FindAsync(id);
        if (playlist != null)
        {
            _db.Playlists.Remove(playlist);
            await _db.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException("Playlist not found");
        }
    }
}
