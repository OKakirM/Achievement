using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

/// <summary>
/// Dados para criar ou atualizar uma review de um jogo.
/// </summary>
public class ReviewCreateDto
{
    /// <summary>
    /// Conteúdo da review (10 a 2000 caracteres).
    /// </summary>
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
    public string ReviewContent { get; set; } = string.Empty;

    /// <summary>
    /// Avaliação final do jogo (0.0 a 10.0).
    /// </summary>
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    [Range(0.0, 10.0, ErrorMessage = "A avaliação deve estar entre {1} e {2}.")]
    public double Rating { get; set; }

    /// <summary>
    /// Id do jogo avaliado.
    /// </summary>
    [Required]
    public int GameId { get; set; }

    /// <summary>
    /// Id do utilizador autor da review.
    /// </summary>
    [Required]
    public int UserId { get; set; }
}
