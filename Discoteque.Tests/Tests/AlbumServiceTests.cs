using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AlbumServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AlbumService _albumService;

    public AlbumServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _albumService = new AlbumService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAlbum_ShouldReturnSuccess()
    {
        // Arrange
        var album = new Album { Name = "Test Album", Year = 2024, Genre = Genres.Rock, Cost = 10000 };
        _unitOfWorkMock.Setup(u => u.ArtistRepository.FindAsync(It.IsAny<int>()))
            .Returns(Task.FromResult(new Artist()));

        // Act
        var result = await _albumService.CreateAlbum(album);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, (int)result.StatusCode);
    }

    [Fact]
    public async Task GetAlbumsAsync_ShouldReturnAlbums()
    {
        IEnumerable<Album> albumList = new List<Album>{
            new Album { Name = "Test Album", Year = 2024, Genre = Genres.Rock, Cost = 10000 }
        };
        // Arrange
        _unitOfWorkMock.Setup(u => u.AlbumRepository.GetAllAsync(null!, x => x.OrderBy(x => x.Id), new Artist().GetType().Name))
            .Returns(Task.FromResult(albumList));

        // Act
        var result = await _albumService.GetAlbumsAsync(false);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateAlbum_ShouldReturnBadRequest_WhenArtistNotFound()
    {
        // Arrange
        var album = new Album { ArtistId = 1, Name = "Test Album", Year = 2023, Cost = 1000 };
        _unitOfWorkMock.Setup(u => u.ArtistRepository.FindAsync(album.ArtistId))
            .Returns(Task.FromResult<Artist>(null));

        // Act
        var result = await _albumService.CreateAlbum(album);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task CreateAlbum_ShouldReturnBadRequest_WhenInvalidYear()
    {
        // Arrange
        var album = new Album { ArtistId = 1, Name = "Test Album", Year = 1800, Cost = 1000 };
        _unitOfWorkMock.Setup(u => u.ArtistRepository.FindAsync(album.ArtistId))
            .Returns(Task.FromResult(new Artist()));

        // Act
        var result = await _albumService.CreateAlbum(album);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task CreateAlbum_ShouldReturnOk_WhenValidAlbum()
    {
        // Arrange
        var album = new Album { ArtistId = 1, Name = "Valid Album", Year = 2023, Cost = 1000 };
        _unitOfWorkMock.Setup(u => u.ArtistRepository.FindAsync(album.ArtistId))
            .Returns(Task.FromResult(new Artist()));

        _unitOfWorkMock.Setup(u => u.AlbumRepository.AddAsync(It.IsAny<Album>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _albumService.CreateAlbum(album);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        _unitOfWorkMock.Verify(u => u.AlbumRepository.AddAsync(It.IsAny<Album>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }
}
