namespace AchievementAPI.Dtos;

/// <summary>
/// Representação de uma plataforma devolvida pela API.
/// </summary>
public class PlatformDto
{
    /// <summary>
    /// Chave primária.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tipo da plataforma (Console, PC, Portable, Mobile, VR).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Nome do aparelho.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
