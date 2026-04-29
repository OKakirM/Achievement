using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Achievement.Models
{
    public class Genre : IValidatableObject
    {
        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do gênero
        /// </summary>
        [Required(ErrorMessage = "O nome do gênero é obrigatório.")]
        [Display(Name = "Nome do Gênero")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O nome do gênero deve ter entre {2} e {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-N, vários gênero pertence a vários jogo
        /// </summary>
        [Display(Name = "Jogos")]
        public ICollection<Game> Games { get; set; } = new List<Game>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Evita nomes compostos apenas por espaços
            if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length < 2)
            {
                yield return new ValidationResult("O nome do gênero é obrigatório e não pode ser vazio.", new[] { nameof(Name) });
            }
        }
    }
}
