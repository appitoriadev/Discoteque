using System.Security.Claims;
using Discoteque.Data.Models;
namespace Discoteque.Business.IServices;

public interface IAuthService
{

    Task<AuthResponse>Register(User user);
    Task<AuthResponse?>Login(LoginRequest request);
    Task<AuthResponse?>RefreshToken(string refreshToken);
    Task<User?> GetUserByUsername(string username);
    Task<User?> GetUserById(int id);
    
} 