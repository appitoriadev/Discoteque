using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

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

    [Fact]
    public async Task CreateUser_ShouldHashPassword()
    {
        // Arrange
        var password = "testpassword";
        var user = new User { Username = "testuser", PasswordHash = password, Email = "mail@test.test" };

        // Act
        var result = await _userService.CreateUser(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(password, result.PasswordHash);
        Assert.True(VerifyPassword(password, result.PasswordHash));
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnUser()
    {
        // Arrange
        var username = "testuser";
        var user = new User { Username = username, PasswordHash = "password", Email = "mail@test.test" };
        _unitOfWorkMock.Setup(u => u.UserRepository.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByUsername(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = Convert.ToBase64String(hashedBytes);
            return hash == hashedPassword;
        }
    }
}
