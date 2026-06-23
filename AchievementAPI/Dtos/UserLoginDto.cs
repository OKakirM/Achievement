using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class UserLoginDto
{
    [Required]
    public string UsernameOrEmail { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}