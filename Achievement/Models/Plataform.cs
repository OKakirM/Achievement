using System.ComponentModel.DataAnnotations;

namespace Achievement.Models
{
    public class Plataform
    {
        public class Plantaforma
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
        }
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
