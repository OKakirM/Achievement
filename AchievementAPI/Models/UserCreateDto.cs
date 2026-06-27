using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

/// <summary>
/// Dados para criar um utilizador. A password é exigida porque a conta é
/// também criada na tabela do ASP.NET Identity (AspNetUsers).
/// </summary>
public class UserCreateDto
{
    /// <summary>Nome do utilizador (2 a 50 caracteres).</summary>
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Endereço de e-mail (único). Liga esta entrada à conta de login no Identity.</summary>
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(150, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [EmailAddress(ErrorMessage = "Formato de {0} inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Palavra-passe: mínimo 8 caracteres, com maiúscula, minúscula, número e caractere especial.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*\-]).{8,}$",
        ErrorMessage = "A Palavra-Passe tem que ter: mínimo 8 caracteres, uma maiúscula, uma minúscula, um número e um caractere especial")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// URL do avatar/foto de perfil (opcional).
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// URL do banner de perfil (opcional).
    /// </summary>
    public string? Banner { get; set; }
}
