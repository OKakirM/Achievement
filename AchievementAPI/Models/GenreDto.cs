namespace AchievementAPI.Dtos;

/// <summary>
/// Representação de um gênero devolvida pela API.
/// </summary>
public class GenreDto
{
    /// <summary>
    /// Chave primária.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do gênero.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
