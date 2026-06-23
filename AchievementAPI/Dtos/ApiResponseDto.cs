namespace AchievementAPI.Dtos;

public class ApiResponseDto<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public string[]? Errors { get; set; }
    public string? Message { get; set; }
}