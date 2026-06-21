using Achievement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // Utilizadores

        /// <summary>
        /// Implementação da tabela Users na Base de Dados
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Implementação da tabela Reviews na Base de Dados
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        // Jogos

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

    }
}
