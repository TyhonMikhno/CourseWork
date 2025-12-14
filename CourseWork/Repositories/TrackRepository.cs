using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using Repositories.Interfaces;

namespace Repositories;

public class TrackRepository : ITrackRepository
{
    private readonly AppDbContext _db;

    public TrackRepository(AppDbContext db) => _db = db;

    public async Task<List<Track>> GetAllAsync()
        => await _db.Tracks.Include(t => t.Album)
                           .ThenInclude(a => a.Artist)
                           .ToListAsync();

    public async Task<Track?> GetByIdAsync(int id)
        => await _db.Tracks.Include(t => t.Album)
                           .ThenInclude(a => a.Artist)
                           .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Track> CreateAsync(Track track)
    {
        var album = await _db.Albums
        .Include(a => a.Tracks)
        .FirstOrDefaultAsync(a => a.Id == track.AlbumId);

    if (album == null)
        throw new KeyNotFoundException("Album not found");

    if (album.Tracks.Count >= album.SongCount)
        throw new InvalidOperationException("Cannot add more tracks than SongCount");

    _db.Tracks.Add(track);
    await _db.SaveChangesAsync();
    return track;
    }

    public async Task UpdateAsync(Track track)
    {
        var existing = await _db.Tracks.FindAsync(track.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException("Track not found");
        }
        else
        {
            _db.Tracks.Update(track);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var track = await _db.Tracks.FindAsync(id);
        if (track != null)
        {
            _db.Tracks.Remove(track);
            await _db.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException("Track not found");
        }
    }
}
