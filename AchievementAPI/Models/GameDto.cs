using System;
using System.Collections.Generic;
using System.Linq;

namespace AchievementAPI.Dtos;

public class GameDto
{
    /// <summary>
    /// Chave Primária
    /// </summary> 
    public int Id { get; set; }

    /// <summary>
    /// Nome do Jogo
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Descrição do jogo
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Data de lançamento do jogo
    /// </summary>
    public DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Data de criação do registo (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Avaliação do Jogo
    /// </summary>
    public double? Rating { get; set; }

    /// <summary>
    /// Duração média do jogo
    /// </summary>
    public double? Length { get; set; }

    /// <summary>
    /// Imagem do jogo (Capa)
    /// </summary>
    public string? CoverImage { get; set; }

    /// <summary>
    /// Imagem do jogo (Banner)
    /// </summary>
    public string? BannerImage { get; set; }

    /// <summary>
    /// Número de vezes que o jogo foi jogado
    /// </summary>
    public int? Plays { get; set; }

    /// <summary>
    /// Plataformas em que o jogo está disponível
    /// </summary>
    public IEnumerable<string> Platforms { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gêneros do jogo
    /// </summary>
    public IEnumerable<string> Genres { get; set; } = Enumerable.Empty<string>();
}
