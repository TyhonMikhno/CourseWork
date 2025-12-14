using System.Text.Json.Serialization;

namespace Models;

public class PlaylistTrack
{
    public int PlaylistId { get; set; }
    [JsonIgnore]
    public Playlist? Playlist { get; set; }

    public int TrackId { get; set; }
    [JsonIgnore]
    public Track? Track { get; set; }

    public int Position { get; set; }
}
