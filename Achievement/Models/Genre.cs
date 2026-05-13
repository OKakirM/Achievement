using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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
        /// - Máximo de 50 caracteres, mínimo de 2 caracteres
        /// </summary>
        [Required(ErrorMessage = "O {0} do gênero é obrigatório.")]
        [Display(Name = "Nome do Gênero")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O {0} do gênero deve ter entre {2} e {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-N, vários gênero pertence a vários jogo
        /// </summary>
        [Display(Name = "Jogos")]
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
