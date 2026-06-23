using System;

namespace AchievementAPI.Dtos;

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Points { get; set; }
    public DateTime CreatedAt { get; set; }
}