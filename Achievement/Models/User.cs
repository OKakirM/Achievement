using System.ComponentModel.DataAnnotations;

namespace Achievement.Models
{
    public class User
    {
        /// <summary>
        /// Chave Primária
        /// </summary> 
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do Utilizador
        /// </summary> 
        [Required]
        [Display(Name = "Nome")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Endereço email do Utilizador
        /// </summary> 
        [Required]
        [Display(Name = "E-mail")]
        [StringLength (150)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password do Utilizador
        /// </summary>
        [Required]
        [Display(Name = "Palavra-Passe")]
        [RegularExpression(pattern: "/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$/", ErrorMessage = "A Palavra-Passe tem que ter: \n - Minimo de 8 caracteres \n - Pelo menos uma letra maiuscula \n - Pelo menos uma letra minuscula \n - Pelo menos um numero \n - Pelo menos uma letra especial")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Imagem/Avatar do Utilizador
        /// - Pode não ser obrigatório
        /// </summary>
        [Display(Name = "Foto de Perfil")]
        public string? Image { get; set; } = string.Empty;

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de 1-N, um utilizador possuem várias review/análise
        /// </summary>
        [Display(Name = "Reviews")]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        /// <summary>
        /// Conexão de N-N, vários utilizador possui vários jogos
        /// </summary>
        [Display(Name = "Jogos")]
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
