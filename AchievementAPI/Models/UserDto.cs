namespace AchievementAPI.Dtos;

/// <summary>
/// Representação pública de um utilizador (sem dados sensíveis como palavra-passe).
/// </summary>
public class UserDto
{
    /// <summary>
    /// Chave primária.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do utilizador.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// URL do avatar/foto de perfil.
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// URL do banner de perfil.
    /// </summary>
    public string? Banner { get; set; }
}
