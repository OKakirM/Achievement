using Achievement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // =======================
        //      Utilizadores
        // =======================

        /// <summary>
        /// Implementação da tabela Users na Base de Dados
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Implementação da tabela Reviews na Base de Dados
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        /// <summary>
        /// Tabela de junção Utilizador-Jogo (a lista, com estado)
        /// </summary>
        public DbSet<UserGame> UserGames { get; set; }

        // =======================
        //         Jogos
        // =======================

        /// <summary>
        /// Implementação da tabela Games na Base de Dados
        /// </summary>
        public DbSet<Game> Games { get; set; }
        /// <summary>
        /// Implementação da tabela Genres na Base de Dados
        /// </summary>
        public DbSet <Genre> Genres { get; set; }
        /// <summary>
        /// Implementação da tabela Platforms na Base de Dados
        /// </summary>
        public DbSet<Platform> Platforms { get; set; }

        /// <summary>
        /// Configurações adicionais da base de dados
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Junção N-N do UsersGame
            builder.Entity<UserGame>().HasKey(ug => new { ug.UserFK, ug.GameFK });
            builder.Entity<UserGame>()
                .HasOne(ug => ug.Game).WithMany(g => g.UserGames).HasForeignKey(ug => ug.GameFK);
            builder.Entity<UserGame>()
                .HasOne(ug => ug.User).WithMany(u => u.UserGames).HasForeignKey(ug => ug.UserFK);

            // Uma review por utilizador por jogo
            builder.Entity<Review>().HasIndex(r => new { r.GameFK, r.UserFK }).IsUnique();
        }

        /// <summary>
        /// Recalcula a nota do jogo como a média das notas das suas reviews.
        /// </summary>
        public async Task RecalculateGameRatingAsync(int gameId)
        {
            var game = await Games.FindAsync(gameId);
            if (game == null) return;

            game.Rating = await Reviews
                .Where(r => r.GameFK == gameId)
                .Select(r => (double?)r.Rating)
                .AverageAsync();
        }

        /// <summary>
        /// Recalcula quantas pessoas jogaram o jogo.
        /// </summary>
        public async Task RecalculateGamePlaysAsync(int gameId)
        {
            var game = await Games.FindAsync(gameId);
            if (game == null) return;

            game.Plays = await UserGames
                .CountAsync(ug => ug.GameFK == gameId && ug.Status != GameStatus.PlanToPlay);
        }
    }
}
