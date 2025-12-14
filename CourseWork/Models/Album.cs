using System.Text.Json.Serialization;

namespace Models;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Length { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
    public int SongCount { get; set; }

    public int ArtistId { get; set; }
    
    [JsonIgnore]
    public Artist? Artist { get; set; } 

    [JsonIgnore]
    public List<Track> Tracks { get; set; } = new();
}