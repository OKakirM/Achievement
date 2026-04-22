namespace Achievement.Models
{
    public class User
    {
        /// <summary>
        /// Chave Primária
        /// </summary> 
        public int Id { get; set; }

        /// <summary>
        /// Nome do Utilizador
        /// </summary> 
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Endereço email do Utilizador
        /// </summary> 
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password do Utilizador
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Imagem/Avatar do Utilizador
        /// - Pode não ser obrigatório
        /// </summary>
        public string? Image { get; set; } = string.Empty;
    }
}
