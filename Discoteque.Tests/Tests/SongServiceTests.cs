using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SongServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SongService _songService;

    public SongServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _songService = new SongService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateSong_ShouldReturnSuccess()
    {
        // Arrange
        var song = new Song { Name = "Test Song", AlbumId = 1 };
        _unitOfWorkMock.Setup(u => u.AlbumRepository.FindAsync(It.IsAny<int>())).ReturnsAsync(new Album());

        // Act
        var result = await _songService.CreateSong(song);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, (int)result.StatusCode);
    }
}
