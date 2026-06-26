using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class GameCreateDto
{
    [Required]
    [StringLength(500)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Range(0, 10)]
    public int? Rating { get; set; }

    [StringLength(100)]
    public string? Length { get; set; }

    [StringLength(1000)]
    public string? CoverImage { get; set; }

    [StringLength(1000)]
    public string? BannerImage { get; set; }

    public int? Plays { get; set; }

    // relacionamentos por Ids (N-N)
    public IEnumerable<int> PlataformIds { get; set; } = Array.Empty<int>();
    public IEnumerable<int> GenreIds { get; set; } = Array.Empty<int>();
}