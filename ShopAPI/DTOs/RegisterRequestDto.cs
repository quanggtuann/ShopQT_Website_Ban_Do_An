using System.ComponentModel.DataAnnotations;

namespace ShopAPI.DTOs;

public class RegisterRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime DateorBirth { get; set; }
}
