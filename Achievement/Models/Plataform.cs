using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Models
{
    public class Plataform
    {
        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tipo da plataforma (Console, PC, Portable ou Mobile)
        /// </summary>
        [Required]
        [Display(Name = "Tipo de Plataforma")]
        public PlataformType Type { get; set; }

        /// <summary>
        /// Nome do aparelho, ou seja, o nome do console, do pc, do portátil ou do dispositivo móvel
        /// </summary>
        [Required]
        [Display(Name = "Nome do Aparelho")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Fabricante (ex: Sony, Microsoft, Nintendo)
        /// </summary>
        [Required]
        [Display(Name = "Fabricante")]
        [StringLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Data de lançamento da plataforma
        /// </summary>
        [Display(Name = "Data de Lançamento")]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Slug amigável para URL (ex: ps5, xbox-series-x)
        /// </summary>
        [Required]
        [Display(Name = "Slug")]
        [StringLength(100)]
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Visibilidade/soft-delete
        /// </summary>
        [Display(Name = "Visível")]
        public bool IsVisible { get; set; } = true;

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-N, várias plataforma pertence á vários um jogo
        /// </summary>
        [Display(Name = "Jogos")]
        public ICollection<Game> Games { get; set; } = new List<Game>();

    }

    /// <summary>
    /// Tipos de plataforma existentes atualmente.
    /// </summary>
    public enum PlataformType

    {
        Console,
        PC,
        Portable,
        Mobile,
        VR
    }
}
