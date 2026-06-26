using System.ComponentModel.DataAnnotations;

namespace AchievementAPI.Dtos;

public class PlataformCreateDto
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
