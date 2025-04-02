using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using System.Threading.Tasks;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userService = new UserService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Username = "testuser", PasswordHash = "password", Email = "mail@test.test" };

        // Act
        var result = await _userService.CreateUser(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }
}
