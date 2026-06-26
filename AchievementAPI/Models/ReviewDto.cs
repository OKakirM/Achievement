namespace AchievementAPI.Dtos;

public class ReviewDto
{
    public int Id { get; set; }
    public string ReviewContent { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
}
