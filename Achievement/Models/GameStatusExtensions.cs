using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Achievement.Models
{
    /// <summary>
    /// Apresentação de um <see cref="GameStatus"/>: nome legível e classe CSS de cor.
    /// Fonte única usada pelas views (reviews, perfil, lista).
    /// </summary>
    public static class GameStatusExtensions
    {
        public static string DisplayName(this GameStatus status)
        {
            var member = typeof(GameStatus).GetMember(status.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? status.ToString();
        }

        public static string CssClass(this GameStatus status) => status switch
        {
            GameStatus.Completed => "status-completed",
            GameStatus.Playing => "status-playing",
            GameStatus.Dropped => "status-dropped",
            GameStatus.OnHold => "status-onhold",
            _ => "status-plan", // PlanToPlay
        };

        public static string IconClass(this GameStatus status) => status switch
        {
            GameStatus.Completed => "bi-check-circle-fill",
            GameStatus.Playing => "bi-play-circle-fill",
            GameStatus.Dropped => "bi-x-circle-fill",
            GameStatus.OnHold => "bi-pause-circle-fill",
            _ => "bi-bookmark-fill", // PlanToPlay
        };
    }
}
