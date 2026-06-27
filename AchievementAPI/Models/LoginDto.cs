using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

/// <summary>
/// Credenciais para obter um token JWT.
/// </summary>
public class LoginDto
{
    /// <summary>E-mail da conta.</summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>Palavra-passe da conta.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
