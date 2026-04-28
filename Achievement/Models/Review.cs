using System.ComponentModel.DataAnnotations;

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
        public string ReviewContent { get; set; } = string.Empty;

        /// <summary>
        /// Avaliação final do jogo pelo utilizador
        /// </summary>
        [Required]
        [Display(Name = "Avaliação")]
        public int Rating { get; set; }
    }
}

