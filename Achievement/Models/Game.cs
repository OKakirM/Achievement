using static Achievement.Models.Plataform;

namespace Achievement.Models
{
    public class Game
    {
        /// <summary>
        /// Chave Primária
        /// </summary> 
        public int Id { get; set; }

        /// <summary>
        /// Nome do Jogo
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do jogo, onde se pode colocar o enredo
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Data de lançamento do jogo
        /// </summary>
        public string ReleaseDate { get; set; } = string.Empty;

        /// <summary>
        /// Avaliação do Jogo
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Duração média do jogo
        /// </summary>
        public string? Length { get; set; } = string.Empty;

        /// <summary>
        /// Imagem do jogo (Capa)
        /// </summary>
        public string? CoverImage { get; set; } = string.Empty;

        /// <summary>
        /// Imagem do jogo (Banner)
        /// </summary>
        public string? BannerImage { get; set; } = string.Empty;

        /// <summary>
        /// Quantas pessoas, na plataforma, jogaram o jogo
        /// </summary>
        public int? Plays { get; set; }
    }
}
