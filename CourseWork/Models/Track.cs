using System.Text.Json.Serialization;

namespace Models;

public class Track
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string Genre { get; set; } = string.Empty;

    public int AlbumId { get; set; }

    [JsonIgnore]
    public Album? Album { get; set; }

    [JsonIgnore]
    public List<PlaylistTrack> PlaylistTracks { get; set; } = new();
}
