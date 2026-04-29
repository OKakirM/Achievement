using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Achievement.Models
{
    public class Review
    {
        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Contéudo da review
        /// </summary>
        [Required]
        [Display(Name = "Review")]
        [StringLength(1000, MinimumLength = 10)]
        public string ReviewContent { get; set; } = string.Empty;

        /// <summary>
        /// Avaliação final do jogo pelo utilizador
        /// </summary>
        [Required]
        [Range(1, 10)]
        [Display(Name = "Avaliação")]
        public int Rating { get; set; }

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-1, várias review/análise pertence á um jogo
        /// </summary>
        [ForeignKey(nameof(Game))]
        [Display(Name = "Jogo")]
        public int GameFK { get; set; }
        [ValidateNever]
        public Game Game { get; set; } = null!;

        /// <summary>
        /// Conexão de N-1, várias review/análise pertence á um utilizadores
        /// </summary>
        [ForeignKey(nameof(User))]
        [Display(Name = "Utilizador")]
        public int UserFK { get; set; }
        [ValidateNever]
        public User User { get; set; } = null!;


    }
}

