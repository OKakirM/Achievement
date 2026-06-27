namespace AchievementAPI.Dtos;

public class UserGameDto
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public Achievement.Models.GameStatus Status { get; set; }

    // Optional helper fields
    public string? UserName { get; set; }
    public string? GameName { get; set; }
}
