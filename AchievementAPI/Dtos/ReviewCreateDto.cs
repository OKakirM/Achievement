using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class ReviewCreateDto
{
    [Required]
    [StringLength(4000)]
    public string ReviewContent { get; set; } = string.Empty;

    [Required]
    [Range(0,10)]
    public int Rating { get; set; }

    [Required]
    public int GameId { get; set; }

    [Required]
    public int UserId { get; set; }
}
