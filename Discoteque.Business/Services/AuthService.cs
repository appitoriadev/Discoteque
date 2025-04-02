using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Discoteque.Business.IServices;
using Discoteque.Data.Models;
using Discoteque.Data;

public class AuthService : IAuthService
{   
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResponse> Register(User user)
    {
        var existingUser = await _unitOfWork.UserRepository.FindAsync(user.Id);
        if (existingUser != null)
            throw new InvalidOperationException("User already exists");

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        var newUser = new User
        {
            Username = user.Username,
            Email = user.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry
        };

        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "30");
        var response = new AuthResponse
        {
            Username = user.Username,
            Token = GenerateJwtToken(user),
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        await _unitOfWork.UserRepository.AddAsync(newUser);
        await _unitOfWork.SaveAsync();

        return response;
    }

    public async Task<AuthResponse?> Login(LoginRequest request)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync(u => u.Username == request.Username);
        var user = users.FirstOrDefault();
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid password");

        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse?> RefreshToken(string refreshToken)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync(u => u.RefreshToken == refreshToken);
        var user = users.FirstOrDefault();
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new InvalidOperationException("Refresh token expired");

        return await GenerateAuthResponse(user);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync(u => u.Username == username);
        return users.FirstOrDefault();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _unitOfWork.UserRepository.FindAsync(id);
    }

    #region Private Methods
    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "30");
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshTokenExpiry;
        await _unitOfWork.SaveAsync();

        var authResponse = new AuthResponse
        {
            Username = user.Username,
            Token = GenerateJwtToken(user),
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };
        return authResponse;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var secretKey = Encoding.UTF8.GetBytes(key);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(secretKey);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "30");
        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
    #endregion
}
