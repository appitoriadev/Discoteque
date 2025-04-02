using System.ComponentModel.DataAnnotations;

namespace Discoteque.Data.Models;
public class AuthResponse: BaseEntity<int>
{
    [Required]
    public required string Token { get; set; }
    [Required]
    public required string RefreshToken { get; set; }
    [Required]
    public required string Username { get; set; }
    [Required]
    public required DateTime ExpiresAt { get; set; }
} 