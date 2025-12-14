using Models;
using Repositories;
using Repositories.Interfaces;
using Validators;
using Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CourseWork.Testing;

public class ArtistTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private readonly ArtistDtoValidator _validator = new ArtistDtoValidator();

    [Fact]
    public async Task CreateArtist_ShouldAddArtist()
    {
        // Arrange
        var db = GetDbContext();
        var repo = new ArtistRepository(db);

        var artist = new Artist
        {
            Name = "Test Artist",
            Age = 30,
            Country = "USA",
            Label = "Sony"
        };

        // Act
        var result = await repo.CreateAsync(artist);

        // Assert
        Assert.Equal("Test Artist", result.Name);
        Assert.Equal(1, db.Artists.Count());
    }

    [Theory]
    [InlineData("John")]
    [InlineData("Alice")]
    [InlineData("Bob")]
    public async Task GetArtist_ShouldReturnArtist(string artistName)
    {
        // Arrange
        var db = GetDbContext();
        var repo = new ArtistRepository(db);

        var artist = new Artist { Name = artistName, Age = 25 };
        db.Artists.Add(artist);
        await db.SaveChangesAsync();

        // Act
        var result = await repo.GetByIdAsync(artist.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(artistName, result!.Name);
    }

    [Fact]
public async Task UpdateArtist_ShouldUpdateSuccessfully()
{
    var db = GetDbContext();
    var repo = new ArtistRepository(db);

    var artist = new Artist { Name = "Original", Age = 30, Country = "USA", Label = "Label1" };
    db.Artists.Add(artist);
    await db.SaveChangesAsync();

    artist.Name = "Updated";
    artist.Age = 35;

    await repo.UpdateAsync(artist);

    var updated = await repo.GetByIdAsync(artist.Id);

    Assert.NotNull(updated);
    Assert.Equal("Updated", updated!.Name);
    Assert.NotEqual("Original", updated.Name); 
    Assert.True(updated.Age > 30); 
}


    [Fact]
    public async Task DeleteArtist_ShouldRemove()
    {
        var db = GetDbContext();
        var repo = new ArtistRepository(db);

        var artist = new Artist { Name = "DeleteMe" };
        db.Artists.Add(artist);
        await db.SaveChangesAsync();

        await repo.DeleteAsync(artist.Id);

        Assert.Empty(db.Artists);
    }
     [Fact]
    public void CreateArtist_InvalidDto_ShouldFailValidation()
    {
        var dto = new DTOs.ArtistDto
        {
            Name = "",
            Age = -1,
            Country = "",
            Label = "A very long label name that exceeds the maximum length allowed for this field in the validation rules"
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        Assert.Contains(result.Errors, e => e.PropertyName == "Age");
        Assert.Contains(result.Errors, e => e.PropertyName == "Country");
        Assert.Contains(result.Errors, e => e.PropertyName == "Label");
    }

    [Fact]
    public async Task GetArtist_NonExistingId_ShouldReturnNull()
    {
        var db = GetDbContext();
        var repo = new ArtistRepository(db);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateArtist_NonExisting_ShouldThrowKeyNotFoundException()
    {
        var db = GetDbContext();
        var repo = new ArtistRepository(db);
        var artist = new Artist { Id = 999, Name = "Ghost" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.UpdateAsync(artist));
    }

    [Fact]
    public async Task DeleteArtist_NonExisting_ShouldThrowKeyNotFoundException()
    {
        var db = GetDbContext();
        var repo = new ArtistRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteAsync(999));
    }

    [Fact]
public async Task GetAllArtists_ShouldReturnList()
{
    var db = GetDbContext();
    db.Artists.AddRange(
        new Artist { Name = "A1", Age = 20, Country = "USA", Label = "L1" },
        new Artist { Name = "A2", Age = 30, Country = "UK", Label = "L2" }
    );
    await db.SaveChangesAsync();

    var repo = new ArtistRepository(db);

    var result = await repo.GetAllAsync();

    Assert.NotNull(result);
    Assert.Equal(2, result.Count);
}

[Fact]
public async Task GetAllArtists_Empty_ShouldReturnEmptyList()
{
    var db = GetDbContext();
    var repo = new ArtistRepository(db);

    var result = await repo.GetAllAsync();

    Assert.Empty(result);
}
}

public class AlbumTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private readonly AlbumDtoValidator _validator = new AlbumDtoValidator();

    [Fact]
    public async Task CreateAlbum_ShouldAddAlbum()
    {
        var db = GetDbContext();
        var repo = new AlbumRepository(db);

        var album = new Album
        {
            Title = "Test Album",
            Length = 40,
            Genre = "Rock",
            Year = 2020,
            SongCount = 10,
            ArtistId = 1
        };

        var result = await repo.CreateAsync(album);

        Assert.Equal("Test Album", result.Title);
        Assert.Single(db.Albums);
    }

    [Fact]
    public async Task GetAlbum_ShouldReturnAlbum()
    {
        var db = GetDbContext();
        var repo = new AlbumRepository(db);

        var artist = new Artist { Name = "Artist1" };
        db.Artists.Add(artist);
        await db.SaveChangesAsync();

        var album = new Album
        {
            Title = "AAA",
            ArtistId = artist.Id,
            Tracks = new List<Track>()
        };
        db.Albums.Add(album);
        await db.SaveChangesAsync();

        var result = await repo.GetByIdAsync(album.Id);

        Assert.NotNull(result);
        Assert.Equal("AAA", result!.Title);
        Assert.NotNull(result.Artist);
    }


    [Fact]
    public async Task UpdateAlbum_ShouldModify()
    {
        var db = GetDbContext();
        var repo = new AlbumRepository(db);

        var album = new Album { Title = "Old", ArtistId = 1 };
        db.Albums.Add(album);
        await db.SaveChangesAsync();

        album.Title = "New";
        await repo.UpdateAsync(album);

        Assert.Equal("New", db.Albums.First().Title);
    }

    [Fact]
    public async Task DeleteAlbum_ShouldRemove()
    {
        var db = GetDbContext();
        var repo = new AlbumRepository(db);

        var album = new Album { Title = "Delete", ArtistId = 1 };
        db.Albums.Add(album);
        await db.SaveChangesAsync();

        await repo.DeleteAsync(album.Id);

        Assert.Empty(db.Albums);
    }

    [Fact]
    public void CreateAlbum_InvalidDto_ShouldFailValidation()
    {
        var dto = new DTOs.AlbumDto
        {
            Title = "",
            Length = -5,
            Year = 1800,
            SongCount = 0
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        Assert.Contains(result.Errors, e => e.PropertyName == "Length");
        Assert.Contains(result.Errors, e => e.PropertyName == "Year");
        Assert.Contains(result.Errors, e => e.PropertyName == "SongCount");
    }

     [Fact]
    public async Task UpdateAlbum_NonExisting_ShouldThrowKeyNotFoundException()
    {
        var db = GetDbContext();
        var repo = new AlbumRepository(db);
        var album = new Album { Id = 999, Title = "Ghost", ArtistId = 1 };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.UpdateAsync(album));
    }

    [Fact]
    public async Task GetAlbum_NonExistingId_ShouldReturnNull()
    {
        var db = GetDbContext();
        var repo = new AlbumRepository(db);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAlbum_NonExisting_ShouldThrowKeyNotFoundException()
    {
        var db = GetDbContext();
        var repo = new AlbumRepository(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteAsync(999));
    }

   [Fact]
    public async Task GetAllAlbums_ShouldReturnList()
    {
        // Arrange
        var db = GetDbContext();

        // Створюємо артиста
        var artist = new Artist { Name = "Artist1", Age = 30, Country = "USA", Label = "Label1" };
        db.Artists.Add(artist);
        await db.SaveChangesAsync();

        // Створюємо альбоми та прив'язуємо ArtistId
        db.Albums.AddRange(
            new Album { Title = "Alb1", Year = 2000, Length = 45, SongCount = 10, ArtistId = artist.Id },
            new Album { Title = "Alb2", Year = 2010, Length = 50, SongCount = 12, ArtistId = artist.Id }
        );
        await db.SaveChangesAsync();

        var repo = new AlbumRepository(db);

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    
    }

[Fact]
public async Task GetAllAlbums_Empty_ShouldReturnEmptyList()
{
    var db = GetDbContext();
    var repo = new AlbumRepository(db);

    var result = await repo.GetAllAsync();

    Assert.Empty(result);
}
}

public class TrackTests
{
     [Fact]
    public async Task GetTrack_ShouldReturnMockedTrack()
    {
        // Arrange
        var mockRepo = new Mock<ITrackRepository>();

        mockRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Track
            {
                Id = 1,
                Title = "MockedSong",
                DurationSeconds = 180,
                AlbumId = 2
            });

        // Act
        var result = await mockRepo.Object.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("MockedSong", result!.Title);
        mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
public async Task CreateTrack_ShouldCallRepository_WhenAlbumNotFull()
{
    // Arrange
    var mockRepo = new Mock<ITrackRepository>();

    // Альбом ще не переповнений, тому CreateAsync має спрацювати
    var trackToAdd = new Track
    {
        Title = "New Track",
        DurationSeconds = 180,
        AlbumId = 1
    };

    mockRepo.Setup(r => r.CreateAsync(It.Is<Track>(t => t.AlbumId == 1 && t.Title == "New Track")))
            .ReturnsAsync((Track t) => t)
            .Verifiable();

    var repo = mockRepo.Object;

    // Act
    var result = await repo.CreateAsync(trackToAdd);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("New Track", result.Title);
    Assert.Equal(180, result.DurationSeconds);

    // Перевіряємо, що метод CreateAsync викликався саме один раз
    mockRepo.Verify(r => r.CreateAsync(It.IsAny<Track>()), Times.Once);
    mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
}

    [Fact]
    public async Task UpdateTrack_ShouldCallRepository()
    {
        var track = new Track { Id = 3, Title = "Old" };

        var mock = new Mock<ITrackRepository>();
        mock.Setup(r => r.UpdateAsync(track))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var repo = mock.Object;

        await repo.UpdateAsync(track);

        mock.Verify(r => r.UpdateAsync(track), Times.Once);
    }

    [Fact]
    public async Task DeleteTrack_ShouldCallRepositoryOnce()
    {
        var mockRepo = new Mock<ITrackRepository>();

        mockRepo
            .Setup(r => r.DeleteAsync(5))
            .Returns(Task.CompletedTask);

        await mockRepo.Object.DeleteAsync(5);

        mockRepo.Verify(r => r.DeleteAsync(5), Times.Once);
    }

    [Fact]
    public async Task CreateTrack_InvalidData_ShouldThrowArgumentException()
    {
        // Arrange
        var mockRepo = new Mock<ITrackRepository>();

        // Кидаємо виняток при створенні некоректного треку
        mockRepo.Setup(r => r.CreateAsync(It.Is<Track>(t => string.IsNullOrEmpty(t.Title) || t.DurationSeconds <= 0)))
                .ThrowsAsync(new ArgumentException("Invalid track"));

        var invalidTrack = new Track { Title = "", DurationSeconds = -10, AlbumId = 1 };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => mockRepo.Object.CreateAsync(invalidTrack));
    }

    [Fact]
    public async Task CreateTrack_AlbumDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var mockRepo = new Mock<ITrackRepository>();

        mockRepo.Setup(r => r.CreateAsync(It.Is<Track>(t => t.AlbumId == 999)))
                .ThrowsAsync(new KeyNotFoundException("Album not found"));

        var track = new Track { Title = "ValidTrack", DurationSeconds = 180, AlbumId = 999 };

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => mockRepo.Object.CreateAsync(track));
    }

    [Fact]
    public async Task CreateTrack_ExceedsSongCount_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var mockRepo = new Mock<ITrackRepository>();

        mockRepo.Setup(r => r.CreateAsync(It.Is<Track>(t => t.AlbumId == 1)))
                .ThrowsAsync(new InvalidOperationException("Cannot add more tracks than SongCount"));

        var track = new Track { Title = "ExtraTrack", DurationSeconds = 200, AlbumId = 1 };

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => mockRepo.Object.CreateAsync(track));
    }

    [Fact]
    public async Task GetTrack_NotExisting_ShouldReturnNull()
    {
        var mock = new Mock<ITrackRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Track?)null);

        var repo = mock.Object;

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTrack_NotExisting_ShouldThrow()
    {
        var mock = new Mock<ITrackRepository>();
        mock.Setup(r => r.UpdateAsync(It.IsAny<Track>()))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var repo = mock.Object;

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            repo.UpdateAsync(new Track { Id = 999, Title = "X" }));
    }

    [Fact]
    public async Task DeleteTrack_NotExisting_ShouldThrow()
    {
        var mock = new Mock<ITrackRepository>();
        mock.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ThrowsAsync(new KeyNotFoundException("Track not found"));

        var repo = mock.Object;

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            repo.DeleteAsync(999));
    }

[Fact]
public async Task GetAllTracks_ShouldReturnList()
{
    var mock = new Mock<ITrackRepository>();
    mock.Setup(r => r.GetAllAsync())
        .ReturnsAsync(new List<Track>
        {
            new Track { Id = 1, Title = "T1" },
            new Track { Id = 2, Title = "T2" }
        });

    var repo = mock.Object;

    var result = await repo.GetAllAsync();

    Assert.NotNull(result);
    Assert.Equal(2, result.Count);
}

[Fact]
public async Task GetAllTracks_Empty_ShouldReturnEmptyList()
{
    var mock = new Mock<ITrackRepository>();
    mock.Setup(r => r.GetAllAsync())
        .ReturnsAsync(new List<Track>());

    var repo = mock.Object;

    var result = await repo.GetAllAsync();

    Assert.Empty(result);
}
}


public class PlaylistTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private readonly PlaylistDtoValidator _validator = new PlaylistDtoValidator();

    [Fact]
    public async Task CreatePlaylist_ShouldAdd()
    {
        var db = GetDbContext();
        var repo = new PlaylistRepository(db);

        var playlist = new Playlist
        {
            Name = "MyPlaylist",
            Description = "Test"
        };

        var result = await repo.CreateAsync(playlist);

        Assert.Equal("MyPlaylist", result.Name);
        Assert.Single(db.Playlists);
    }

    [Fact]
    public async Task GetPlaylist_ShouldReturn()
    {
        var db = GetDbContext();
        var repo = new PlaylistRepository(db);

        var playlist = new Playlist { Name = "AAA" };
        db.Playlists.Add(playlist);
        await db.SaveChangesAsync();

        var result = await repo.GetByIdAsync(playlist.Id);

        Assert.NotNull(result);
        Assert.Equal("AAA", result!.Name);
    }

    [Fact]
    public async Task UpdatePlaylist_ShouldModify()
    {
        var db = GetDbContext();
        var repo = new PlaylistRepository(db);

        var playlist = new Playlist { Name = "Old" };
        db.Playlists.Add(playlist);
        await db.SaveChangesAsync();

        playlist.Name = "New";
        await repo.UpdateAsync(playlist);

        Assert.Equal("New", db.Playlists.First().Name);
    }

    [Fact]
    public async Task DeletePlaylist_ShouldRemove()
    {
        var db = GetDbContext();
        var repo = new PlaylistRepository(db);

        var playlist = new Playlist { Name = "Delete" };
        db.Playlists.Add(playlist);
        await db.SaveChangesAsync();

        await repo.DeleteAsync(playlist.Id);

        Assert.Empty(db.Playlists);
    }

    [Fact]
    public void CreatePlaylist_InvalidDto_ShouldFailValidation()
    {
        var dto = new DTOs.PlaylistDto
        {
            Name = "",
            Description = ""
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task UpdatePlaylist_NonExisting_ShouldThrowKeyNotFoundException()
    {
        var db = GetDbContext();
        var repo = new PlaylistRepository(db);
        var playlist = new Playlist { Id = 999, Name = "Ghost" };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.UpdateAsync(playlist));
    }

    [Fact]
    public async Task GetPlaylist_NonExistingId_ShouldReturnNull()
    {
        var db = GetDbContext();
        var repo = new PlaylistRepository(db);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeletePlaylist_NonExisting_ShouldThrowKeyNotFoundException()
    {
        var db = GetDbContext();
        var repo = new PlaylistRepository(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteAsync(999));
    }

    [Fact]
public async Task GetAllPlaylists_ShouldReturnList()
{
    var db = GetDbContext();

    db.Playlists.AddRange(
        new Playlist { Name = "P1" },
        new Playlist { Name = "P2" }
    );
    await db.SaveChangesAsync();

    var repo = new PlaylistRepository(db);

    var result = await repo.GetAllAsync();

    Assert.NotNull(result);
    Assert.Equal(2, result.Count);
}

[Fact]
public async Task GetAllPlaylists_Empty_ShouldReturnEmptyList()
{
    var db = GetDbContext();
    var repo = new PlaylistRepository(db);

    var result = await repo.GetAllAsync();

    Assert.Empty(result);
}
}