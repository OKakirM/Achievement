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
        /// - Máximo de 2000 caracteres, mínimo de 10 caracteres
        /// </summary>
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Review")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
        public string ReviewContent { get; set; } = string.Empty;

        /// <summary>
        /// Avaliação final do jogo pelo utilizador
        /// - Deve estar entre 0.0 e 10.0
        /// </summary>
        [Required(ErrorMessage = "A {0} é obrigatória.")]
        [Display(Name = "Avaliação")]
        [Range(0.0, 10.0, ErrorMessage = "A avaliação deve estar entre {1} e {2}.")]
        public double Rating { get; set; }

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

