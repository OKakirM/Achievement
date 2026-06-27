using System;

namespace AchievementAPI.Dtos;

/// <summary>
/// Representação de uma review devolvida pela API.
/// </summary>
public class ReviewDto
{
    /// <summary>
    /// Chave primária.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Conteúdo da review.
    /// </summary>
    public string ReviewContent { get; set; } = string.Empty;

    /// <summary>
    /// Avaliação final do jogo (0.0 a 10.0).
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Data de criação (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Id do jogo avaliado.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Id do utilizador autor.
    /// </summary>
    public int UserId { get; set; }
}
