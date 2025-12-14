using System.Text.Json.Serialization;

namespace Models;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    [JsonIgnore]
    public List<PlaylistTrack> PlaylistTracks { get; set; } = new();
}
