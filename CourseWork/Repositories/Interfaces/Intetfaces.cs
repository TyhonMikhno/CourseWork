using Models;

namespace Repositories.Interfaces;

public interface IArtistRepository : IRepository<Artist> { }
public interface IAlbumRepository : IRepository<Album> { }
public interface ITrackRepository : IRepository<Track> { }
public interface IPlaylistRepository : IRepository<Playlist> { }
