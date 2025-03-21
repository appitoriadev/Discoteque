using System.ComponentModel.DataAnnotations;

namespace Discoteque.Data.Models;

public class User : BaseEntity<int>
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User"; // Default role
} 