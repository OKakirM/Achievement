using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class AchievementUpdateDto
{
    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(0, 10000)]
    public int? Points { get; set; }
}