using System.ComponentModel.DataAnnotations;

namespace Discoteque.Data.Models;
public class LoginRequest: BaseEntity<int>
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string Password { get; set; }
}