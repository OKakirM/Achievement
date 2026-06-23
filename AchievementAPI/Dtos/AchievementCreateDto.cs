using System;
using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class AchievementCreateDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(0, 10000)]
    public int Points { get; set; }

    public Guid? CategoryId { get; set; }
}