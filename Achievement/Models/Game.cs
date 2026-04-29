using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static Achievement.Models.Plataform;

namespace Achievement.Models
{
    public class Game : IValidatableObject
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
        [Range(0, 10, ErrorMessage = "A avaliação deve estar entre {1} e {2}.")]
        public int? Rating { get; set; }

        /// <summary>
        /// Duração média do jogo
        /// </summary>
        [Display(Name = "Duração Média")]
        [StringLength(100, ErrorMessage = "A duração não pode exceder {1} caracteres.")]
        public string? Length { get; set; } = string.Empty;

        /// <summary>
        /// Imagem do jogo (Capa)
        /// </summary>
        [Display(Name = "Imagem de Capa")]
        [Url(ErrorMessage = "A imagem de capa deve ser uma URL válida.")]
        public string? CoverImage { get; set; } = string.Empty;

        /// <summary>
        /// Imagem do jogo (Banner)
        /// </summary>
        [Display(Name = "Imagem de Banner")]
        [Url(ErrorMessage = "A imagem de banner deve ser uma URL válida.")]
        public string? BannerImage { get; set; } = string.Empty;

        /// <summary>
        /// Quantas pessoas, na plataforma, jogaram o jogo
        /// </summary>
        [Display(Name = "Jogadores que jogaram")]
        [Range(0, int.MaxValue, ErrorMessage = "O número de jogadores não pode ser negativo.")]
        public int? Plays { get; set; }

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-N, vários jogo possui várias plataformas
        /// </summary>
        [Display(Name = "Plataformas")]
        public ICollection<Plataform> Plataforms { get; set; } = new List<Plataform>();

        /// <summary>
        /// Conexão de N-N, vários jogo possui vários gêneros
        /// </summary>
        [Display(Name = "Gêneros")]
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        /// <summary>
        /// Conexão de 1-N, um jogo possui várias reviews/análises
        /// </summary>
        [Display(Name = "Reviews")]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        /// <summary>
        /// Conexão de N-N, vários jogo possuem vários utilizadores
        /// </summary>
        [Display(Name = "Utilizadores")]
        public ICollection<User> Users { get; set; } = new List<User>();

        // Validações que envolvem múltiplas propriedades ou regras não cobertas por atributos
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // ReleaseDate não pode ser no futuro
            if (ReleaseDate > DateTime.Today)
            {
                yield return new ValidationResult("A data de lançamento não pode ser no futuro.", new[] { nameof(ReleaseDate) });
            }

            // Rating já tem Range, mas validar nullability/consistência adicional
            if (Rating.HasValue && (Rating < 0 || Rating > 10))
            {
                yield return new ValidationResult("A avaliação deve estar entre 0 e 10.", new[] { nameof(Rating) });
            }

            // Plays não pode ser negativo (atributo Range cobre, mas redundância segura)
            if (Plays.HasValue && Plays < 0)
            {
                yield return new ValidationResult("O número de jogadores não pode ser negativo.", new[] { nameof(Plays) });
            }
        }
    }
}
