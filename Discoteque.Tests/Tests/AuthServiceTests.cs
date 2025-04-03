using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;
    private readonly string _testJwtKey = "your-256-bit-secret-your-256-bit-secret-your-256-bit-secret";

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();
        
        var configurationSection = new Mock<IConfigurationSection>();
        configurationSection.Setup(x => x.Value).Returns(_testJwtKey);
        
        _configurationMock.Setup(x => x.GetSection("Jwt:Key"))
            .Returns(configurationSection.Object);
            
        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnAuthResponse()
    {
        // Arrange
        var user = new User { Username = "testuser", PasswordHash = "password", Email = "mail@test.test" };
        _unitOfWorkMock.Setup(u => u.UserRepository.FindAsync(It.IsAny<int>())).ReturnsAsync((User)null!);

        // Act
        var result = await _authService.Register(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var user = new User { Username = "testuser", PasswordHash = "password", Email = "mail@test.test" };
        var loginRequest = new LoginRequest { Username = user.Username, Password = user.PasswordHash };
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetAllAsync(
            It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(user)),
            null!,
            ""))
            .ReturnsAsync(new List<User> { user });

        // Act
        var result = await _authService.Login(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.True(ValidateToken(result.Token));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "invaliduser", Password = "wrongpassword" };
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetAllAsync(
            It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(new User { 
                Username = loginRequest.Username, 
                PasswordHash = "hashedpassword",
                Email = "test@test.com"
            })),
            null!,
            ""))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _authService.Login(loginRequest);

        // Assert
        Assert.Null(result);
    }

    private bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_testJwtKey);
        
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
