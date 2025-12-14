namespace Models;

public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    public List<Album> Albums { get; set; } = new();
}
