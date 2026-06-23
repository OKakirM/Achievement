using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class UserRegisterDto
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Compare("Password")]
    public string? ConfirmPassword { get; set; }
}