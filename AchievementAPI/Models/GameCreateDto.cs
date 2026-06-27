using Achievement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class GameCreateDto
{
    /// <summary>
    /// Nome do Jogo
    /// - Máximo de 100 caracteres, mínimo de 2 caracteres
    /// - Obrigatório
    /// </summary>
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do jogo, onde se pode colocar o enredo
    /// - Máximo de 2000 caracteres, mínimo de 10 caracteres
    /// - Obrigatório
    /// </summary>
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    [Display(Name = "Descrição")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data de lançamento do jogo
    /// - Obrigatório
    /// </summary>
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    [Display(Name = "Data de Lançamento")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Avaliação do Jogo
    /// - Deve estar entre 0.0 e 10.0
    /// </summary>
    [Display(Name = "Avaliação")]
    [Range(0.0, 10.0, ErrorMessage = "A avaliação deve estar entre {1} e {2}.")]
    public double? Rating { get; set; }

    /// <summary>
    /// Número de jogadores que já jogaram o jogo
    /// - Não pode ser negativo
    /// </summary>
    [Display(Name = "Jogadores que jogaram")]
    [Range(0, int.MaxValue, ErrorMessage = "O número de jogadores não pode ser negativo.")]
    public int? Plays { get; set; }

    /// <summary>
    /// Duração média do jogo
    /// - Deve ser um valor positivo, representando o número de horas que um jogador médio leva para completar o jogo
    /// </summary>
    [Display(Name = "Duração Média")]
    [Range(0.0, 10000.0, ErrorMessage = "A duração tem que ser um valor positivo.")]
    public double? Length { get; set; }

    /// <summary>
    /// Imagem do jogo (Capa)
    /// </summary>
    [StringLength(1000)]
    public string? CoverImage { get; set; }


    /// <summary>
    /// Imagem do jogo (Banner)
    /// </summary>
    [StringLength(1000)]
    public string? BannerImage { get; set; }

    // ============================================
    // Relacionamentos
    // ============================================

    /// <summary>
    /// Conexão de N-N, vários jogo possui várias Plataformas
    /// </summary>
    [Display(Name = "Plataformas")]
    public IEnumerable<int> PlatformIds { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Conexão de N-N, vários jogo possui vários gêneros
    /// </summary>
    [Display(Name = "Gêneros")]
    public IEnumerable<int> GenreIds { get; set; } = Array.Empty<int>();
}