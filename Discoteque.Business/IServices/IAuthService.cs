using Discoteque.Data.Dto;

namespace Discoteque.Business.IServices;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    string GenerateJwtToken(string username, string role);
} 