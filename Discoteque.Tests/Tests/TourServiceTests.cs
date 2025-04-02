using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TourServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TourService _tourService;

    public TourServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tourService = new TourService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateTour_ShouldReturnSuccess()
    {
        // Arrange
        var tour = new Tour { Name = "Test Tour", ArtistId = 1 };
        _unitOfWorkMock.Setup(u => u.ArtistRepository.FindAsync(It.IsAny<int>())).ReturnsAsync(new Artist());

        // Act
        var result = await _tourService.CreateTour(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, (int)result.StatusCode);
    }
}
