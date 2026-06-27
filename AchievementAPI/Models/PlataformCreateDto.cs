using Achievement.Models;
using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

/// <summary>
/// Dados para criar ou atualizar uma plataforma.
/// </summary>
public class PlatformCreateDto
{
    /// <summary>
    /// Tipo da plataforma. Valores aceites: Console, PC, Portable, Mobile, VR.
    /// </summary>
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [EnumDataType(typeof(PlatformType), ErrorMessage = "Tipo de Platforma inválido")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Nome do aparelho (2 a 50 caracteres).
    /// </summary>
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    public string Name { get; set; } = string.Empty;
}
