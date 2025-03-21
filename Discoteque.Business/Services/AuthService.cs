using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Discoteque.Data;
using Discoteque.Data.Dto;
using Discoteque.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Discoteque.Business.IServices;

namespace Discoteque.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // In a real application, you would hash the password and compare with stored hash
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Username == request.Username);
        
        if (user == null || user.PasswordHash != request.Password) // This is simplified - use proper password hashing
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var token = GenerateJwtToken(user.Username, user.Role);

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Username == request.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.Password, // In a real application, hash the password
            Role = "User"
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveAsync();

        var token = GenerateJwtToken(user.Username, user.Role);

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }

    public string GenerateJwtToken(string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 