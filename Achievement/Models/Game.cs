using System.ComponentModel.DataAnnotations;
using static Achievement.Models.Plataform;

namespace Achievement.Models
{
    public class Game
    {
        /// <summary>
        /// Chave Primária
        /// </summary> 
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do Jogo
        /// </summary>
        [Required]
        [Display(Name = "Nome do Jogo")]
        [StringLength(500)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do jogo, onde se pode colocar o enredo
        /// </summary>
        [Required]
        [Display(Name = "Descrição")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Data de lançamento do jogo
        /// </summary>
        [Required]
        [Display(Name = "Data de Lançamento")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Avaliação do Jogo
        /// </summary>
        [Display(Name = "Avaliação")]
        public int? Rating { get; set; }

        /// <summary>
        /// Duração média do jogo
        /// </summary>
        [Display(Name = "Duração Média")]
        public string? Length { get; set; } = string.Empty;

        /// <summary>
        /// Imagem do jogo (Capa)
        /// </summary>
        [Display(Name = "Imagem de Capa")]
        public string? CoverImage { get; set; } = string.Empty;

        /// <summary>
        /// Imagem do jogo (Banner)
        /// </summary>
        [Display(Name = "Imagem de Banner")]
        public string? BannerImage { get; set; } = string.Empty;

        /// <summary>
        /// Quantas pessoas, na plataforma, jogaram o jogo
        /// </summary>
        [Display(Name = "Jogadores que jogaram")]
        public int? Plays { get; set; }
    }
}
