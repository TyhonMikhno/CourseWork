namespace DTOs;

public class AlbumDto
{
    public string? Title { get; set; }
    public double Length { get; set; }           
    public string? Genre { get; set; }
    public int Year { get; set; }
    public int SongCount { get; set; }

    public int ArtistId { get; set; }
}
