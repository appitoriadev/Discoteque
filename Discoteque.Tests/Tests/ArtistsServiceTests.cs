using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ArtistsServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ArtistsService _artistsService;

    public ArtistsServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _artistsService = new ArtistsService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateArtist_ShouldReturnSuccess()
    {
        // Arrange
        var artist = new Artist { Name = "Test Artist" };

        // Act
        var result = await _artistsService.CreateArtist(artist);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, (int)result.StatusCode);
    }

    [Fact]
    public async Task GetArtistsAsync_ShouldReturnArtists()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.ArtistRepository.GetAllAsync()).ReturnsAsync(new List<Artist>());

        // Act
        var result = await _artistsService.GetArtistsAsync();

        // Assert
        Assert.NotNull(result);
    }
}
