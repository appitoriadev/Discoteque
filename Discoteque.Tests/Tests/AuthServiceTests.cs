using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();
        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnAuthResponse()
    {
        // Arrange
        var user = new User { Username = "testuser", PasswordHash = "password", Email = "mail@test.test" };
        _unitOfWorkMock.Setup(u => u.UserRepository.FindAsync(It.IsAny<int>())).ReturnsAsync((User)null);

        // Act
        var result = await _authService.Register(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }
}
