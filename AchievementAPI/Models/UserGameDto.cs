namespace AchievementAPI.Dtos;

/// <summary>
/// Entrada da biblioteca de um utilizador: o estado de um jogo (entidade de junção N-N).
/// </summary>
public class UserGameDto
{
    /// <summary>
    /// Id do utilizador.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Id do jogo.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Estado do jogo para o utilizador (Completed, Playing, Dropped, OnHold, PlanToPlay).
    /// </summary>
    public Achievement.Models.GameStatus Status { get; set; }

    /// <summary>
    /// Nome do utilizador.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Nome do jogo.
    /// </summary>
    public string? GameName { get; set; }
}
