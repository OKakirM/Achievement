using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Achievement.Models
{
    public class Game
    {
        /// <summary>
        /// Chave Primária
        /// </summary> 
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do Jogo
        /// - Máximo de 100 caracteres, mínimo de 2 caracteres
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "O {0} é obrigatório.")]
        [Display(Name = "Nome do Jogo")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do jogo, onde se pode colocar o enredo
        /// - Máximo de 2000 caracteres, mínimo de 10 caracteres
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "A {0} é obrigatória.")]
        [Display(Name = "Descrição")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Data de lançamento do jogo
        /// - Obrigatório
        /// </summary>
        [Required(ErrorMessage = "A {0} é obrigatória.")]
        [Display(Name = "Data de Lançamento")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Data de criação do registo (UTC). Usada para ordenar o feed de "adicionados recentemente".
        /// </summary>
        [Display(Name = "Adicionado em")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Avaliação do Jogo
        /// - Deve estar entre 0.0 e 10.0
        /// </summary>
        [Display(Name = "Avaliação")]
        [Range(0.0, 10.0, ErrorMessage = "A avaliação deve estar entre {1} e {2}.")]
        public double? Rating { get; set; }

        /// <summary>
        /// Duração média do jogo
        /// - Deve ser um valor positivo, representando o número de horas que um jogador médio leva para completar o jogo
        /// </summary>
        [Display(Name = "Duração Média")]
        [Range(0.0, 10000.0, ErrorMessage = "A duração tem que ser um valor positivo.")]
        public double? Length { get; set; }

        /// <summary>
        /// Imagem do jogo (Capa)
        /// </summary>
        [Display(Name = "Imagem de Capa")]
        public string? CoverImage { get; set; }

        /// <summary>
        /// Imagem do jogo (Banner)
        /// </summary>
        [Display(Name = "Imagem de Banner")]
        public string? BannerImage { get; set; }

        /// <summary>
        /// Quantas pessoas, na Platforma, jogaram o jogo
        /// - Deve ser um valor positivo, representando o número de jogadores que jogaram o jogo
        /// </summary>
        [Display(Name = "Jogadores que jogaram")]
        [Range(0, int.MaxValue, ErrorMessage = "O número de jogadores não pode ser negativo.")]
        public int? Plays { get; set; }

        /// <summary>
        /// Desenvolvedora
        /// - Maximo de 200 caracteres, mínimo de 2 caracteres
        /// ==================================================
        /// - TODO:
        /// - - Uma nova tabela para as desenvolvedoras, onde cada jogo tem um FK para a desenvolvedora, e a desenvolvedora tem uma coleção de jogos
        /// </summary>
        [Display(Name = "Desenvolvedora")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres.")]
        public string? Developer { get; set; }

        /// <summary>
        /// URL amigável / slug
        /// - Máximo de 200 caracteres, mínimo de 2 caracteres
        /// </summary>
        [Display(Name = "Slug")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "O Slug só pode conter letras minúsculas, números e hífens.")]
        public string? Slug { get; set; }

        // ============================================
        // Chaves Estrangeiras | Relacionamentos
        // ============================================

        /// <summary>
        /// Conexão de N-N, vários jogo possui várias Plataformas
        /// </summary>
        [Display(Name = "Platformas")]
        public ICollection<Platform> Platforms { get; set; } = new List<Platform>();

        /// <summary>
        /// Conexão de N-N, vários jogo possui vários gêneros
        /// </summary>
        [Display(Name = "Gêneros")]
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        /// <summary>
        /// Conexão de 1-N, um jogo possui várias reviews/análises
        /// </summary>
        [Display(Name = "Reviews")]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        /// <summary>
        /// Conexão de N-N, via entidade de junção UserGame.
        /// </summary>
        [Display(Name = "Utilizadores")]
        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();
    }
}
