using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

/// <summary>
/// Dados para criar ou atualizar um gênero.
/// </summary>
public class GenreCreateDto
{
    /// <summary>
    /// Nome do gênero (2 a 50 caracteres).
    /// </summary>
    [Required(ErrorMessage = "O {0} do gênero é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
    public string Name { get; set; } = string.Empty;
}
