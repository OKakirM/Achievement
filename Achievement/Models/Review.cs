namespace Achievement.Models
{
    public class Review
    {
        /// <summary>
        /// Chave Primária
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Contéudo da review
        /// </summary>
        public string ReviewContent { get; set; } = string.Empty;

        /// <summary>
        /// Avaliação final do jogo pelo utilizador
        /// </summary>
        public int Rating { get; set; }
    }
}

