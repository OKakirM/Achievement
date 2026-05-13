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
        /// - Máximo de 50 caracteres, mínimo de 2 caracteres
        /// - Obrigatório
        /// </summary> 
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Nome")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Endereço email do Utilizador
        /// - Máximo de 150 caracteres
        /// - Formato de email válido
        /// - Obrigatório
        /// </summary> 
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "E-mail")]
        [StringLength (150, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
        [EmailAddress(ErrorMessage = "Formato de {0} inválido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password do Utilizador
        /// - Máximo de 100 caracteres, mínimo de 8 caracteres
        /// - Deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "A {0} é obrigatória.")]
        [Display(Name = "Palavra-Passe")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A {0} deve ter no mínimo {2} caracteres")]
        [RegularExpression(
            pattern: "/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$/", 
            ErrorMessage = "A {0} tem que ter: \n - Minimo de 8 caracteres \n - Pelo menos uma letra maiuscula \n - Pelo menos uma letra minuscula \n - Pelo menos um numero \n - Pelo menos uma letra especial")]
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
