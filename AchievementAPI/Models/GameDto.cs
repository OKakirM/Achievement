using System;
using System.Collections.Generic;
using System.Linq;

namespace AchievementAPI.Dtos;

public class GameDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public IEnumerable<string> Genres { get; set; } = Enumerable.Empty<string>();

    public DateTime ReleaseDate { get; set; }

    public int? Rating { get; set; }

    public string? Length { get; set; }

    public string? CoverImage { get; set; }

    public string? BannerImage { get; set; }

    public int? Plays { get; set; }

    public IEnumerable<string> Platforms { get; set; } = Enumerable.Empty<string>();
}
