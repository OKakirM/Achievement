using System.ComponentModel.DataAnnotations;

namespace Achievement.Models
{
    public class Genre
    {
        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do gênero
        /// </summary>
        [Required]
        [Display(Name = "Nome do Gênero")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
