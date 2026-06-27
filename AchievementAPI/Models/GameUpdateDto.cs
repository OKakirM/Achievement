using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

/// <summary>
/// Atualização parcial de um jogo. Todos os campos são opcionais:
/// apenas os fornecidos (não nulos) são aplicados.
/// </summary>
public class GameUpdateDto
{
    /// <summary>
    /// Nome do Jogo (2 a 100 caracteres).
    /// </summary>
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    /// <summary>
    /// Descrição do jogo (10 a 2000 caracteres).
    /// </summary>
    [StringLength(2000, MinimumLength = 10)]
    public string? Description { get; set; }

    /// <summary>
    /// Data de lançamento do jogo.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// Avaliação do jogo (0.0 a 10.0).
    /// </summary>
    [Range(0.0, 10.0)]
    public double? Rating { get; set; }

    /// <summary>
    /// Duração média em horas (valor positivo).
    /// </summary>
    [Range(0.0, 10000.0)]
    public double? Length { get; set; }

    /// <summary>
    /// URL da imagem de capa.
    /// </summary>
    [StringLength(1000)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// URL da imagem de banner.
    /// </summary>
    [StringLength(1000)]
    public string? BannerImage { get; set; }

    /// <summary>
    /// Número de jogadores que já jogaram o jogo.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? Plays { get; set; }

    /// <summary>
    /// Ids das plataformas (relação N-N). Quando fornecido, substitui o conjunto atual.
    /// </summary>
    public IEnumerable<int>? PlatformIds { get; set; }

    /// <summary>
    /// Ids dos gêneros (relação N-N). Quando fornecido, substitui o conjunto atual.
    /// </summary>
    public IEnumerable<int>? GenreIds { get; set; }
}
