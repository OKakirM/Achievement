using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Achievement.Models
{
    /// <summary>
    /// Entidade de junção N-N entre Utilizador e Jogo.
    /// - Guarda o estado em que o jogo está para esse utilizador.
    /// </summary>
    public class UserGame
    {
        [Display(Name = "Estado")]
        public GameStatus Status { get; set; }

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-1, Vários Jogos pertence a um utilizador
        /// </summary>
        [Display(Name = "Utilizador")]
        public int UserFK { get; set; }
        [ValidateNever]
        public User User { get; set; } = null!;


        /// <summary>
        /// Conexão de N-1, Varios Utilizadores pertence a um jogo
        /// </summary>
        [Display(Name = "Jogo")]
        public int GameFK { get; set; }
        [ValidateNever]
        public Game Game { get; set; } = null!;
    }

    /// <summary>
    /// Estado de um jogo na lista do utilizador.
    /// </summary>
    public enum GameStatus
    {
        [Display(Name = "Concluído")]
        Completed,
        [Display(Name = "A Jogar")]
        Playing,
        [Display(Name = "Abandonado")]
        Dropped,
        [Display(Name = "Em Espera")]
        OnHold,
        [Display(Name = "Pretendo Jogar")]
        PlanToPlay
    }
}
