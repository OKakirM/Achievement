using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Models
{
    public class Platform
    {
        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tipo da Plataforma (Console, PC, Portable ou Mobile)
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Tipo de Platforma")]
        [EnumDataType(typeof(PlatformType), ErrorMessage = "Tipo de Platforma inválido")]
        public PlatformType Type { get; set; }

        /// <summary>
        /// Nome do aparelho, ou seja, o nome do console, do pc, do portátil ou do dispositivo móvel
        /// - Máximo de 50 caracteres, mínimo de 2 caracteres
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Nome do Aparelho")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
        public string Name { get; set; } = string.Empty;

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-N, várias Plataforma pertence á vários um jogo
        /// </summary>
        [Display(Name = "Jogos")]
        public ICollection<Game> Games { get; set; } = new List<Game>();

    }

    /// <summary>
    /// Tipos de Plataforma existentes atualmente.
    /// </summary>
    public enum PlatformType

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

    /// <summary>
    /// Mapeia cada tipo de plataforma para a respetiva classe de Bootstrap Icons.
    /// </summary>
    public static class PlatformTypeIcon
    {
        public static string IconClass(this PlatformType type) => type switch
        {
            PlatformType.Console => "bi-controller",
            PlatformType.PC => "bi-mouse",
            PlatformType.Portable => "bi-nintendo-switch",
            PlatformType.Mobile => "bi-phone",
            PlatformType.VR => "bi-headset-vr",
            _ => "bi-question-circle"
        };

        /// <summary>
        /// Nome localizado do tipo (DisplayAttribute), ex.: Console → "Consola".
        /// </summary>
        public static string DisplayName(this PlatformType type)
        {
            var member = typeof(PlatformType).GetMember(type.ToString());
            var display = member.Length > 0
                ? member[0].GetCustomAttribute<DisplayAttribute>()?.Name
                : null;
            return display ?? type.ToString();
        }
    }
}
