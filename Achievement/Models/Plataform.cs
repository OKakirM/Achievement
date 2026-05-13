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
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Tipo de Plataforma")]
        [EnumDataType(typeof(PlataformType), ErrorMessage = "Tipo de Plataforma inválido")]
        public PlataformType Type { get; set; }

        /// <summary>
        /// Nome do aparelho, ou seja, o nome do console, do pc, do portátil ou do dispositivo móvel
        /// - Máximo de 50 caracteres, mínimo de 2 caracteres
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Nome do Aparelho")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Data de lançamento da plataforma
        /// </summary>
        [Required(ErrorMessage = "A {0} é obrigatória.")]
        [Display(Name = "Data de Lançamento")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

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
        [Display(Name = "Consola")]
        Console,
        [Display(Name = "PC")]
        PC,
        [Display(Name = "Portátil")]
        Portable,
        [Display(Name = "Móvel")]
        Mobile,
        [Display(Name = "Realidade Virtual")]
        VR
    }
}
