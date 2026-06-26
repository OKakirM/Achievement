using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class GenreCreateDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
