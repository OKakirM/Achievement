using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Achievement.Pages.Games
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // Para obter informações do utilizador logado

        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Game Game { get; set; } = default!;

        // Entrada do utilizador logado para este jogo
        public Review? MyReview { get; set; }
        public UserGame? MyEntry { get; set; }

        // Campos do formulário de review
        [BindProperty]
        [Range(0.0, 10.0, ErrorMessage = "A avaliação deve estar entre {1} e {2}.")]
        public double ReviewRating { get; set; }
        [BindProperty]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
        public string? ReviewText { get; set; }

        // Campo do formulário de estado na lista
        [BindProperty]
        public GameStatus Status { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            if (!await LoadGameAsync(id.Value, currentUser))
            {
                return NotFound();
            }

            // Se o slug na URL não corresponder ao slug real do jogo, redireciona para a URL correta.
            if (string.IsNullOrEmpty(slug) || slug != Game.Slug)
            {
                return RedirectToPagePermanent("./Details", new { id = Game.Id, slug = Game.Slug });
            }

            // Popula os formulários com o que o utilizador já tem
            if (MyReview != null)
            {
                ReviewRating = MyReview.Rating;
                ReviewText = MyReview.ReviewContent;
            }
            if (MyEntry != null)
            {
                Status = MyEntry.Status;
            }

            return Page();
        }

        /// <summary>
        /// Adiciona ou atualiza a review do utilizador logado para este jogo. Se não existir, cria uma nova entrada.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostReviewAsync(int id, string? slug)
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser == null)
            {
                return Challenge();
            }

            if (ReviewRating < 0.0 || ReviewRating > 10.0)
            {
                ModelState.AddModelError("ReviewRating", "A nota deve estar entre 0 e 10.");
            }
            if (string.IsNullOrWhiteSpace(ReviewText) || ReviewText.Length < 10 || ReviewText.Length > 2000)
            {
                ModelState.AddModelError("ReviewText", "A review deve ter entre 10 e 2000 caracteres.");
            }

            if (!ModelState.IsValid)
            {
                if (!await LoadGameAsync(id, currentUser))
                {
                    return NotFound();
                }
                return Page();
            }

            // Apenas uma review pode existir por utilizador por jogo
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.GameFK == id && r.UserFK == currentUser.Id);

            if (review == null)
            {
                _context.Reviews.Add(new Review
                {
                    GameFK = id,
                    UserFK = currentUser.Id,
                    Rating = ReviewRating,
                    ReviewContent = ReviewText!
                });
            }
            else
            {
                review.Rating = ReviewRating;
                review.ReviewContent = ReviewText!;
            }

            await _context.SaveChangesAsync();

            // Recalcula a nota do jogo como a média das notas das suas reviews
            await _context.RecalculateGameRatingAsync(id);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id, slug });
        }

        /// <summary>
        /// Atualiza o estado do jogo na lista do utilizador logado. Se não existir, cria uma nova entrada.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostStatusAsync(int id, string? slug)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Challenge();
            }

            var entry = await _context.UserGames.FindAsync(currentUser.Id, id);
            if (entry == null)
            {
                _context.UserGames.Add(new UserGame { UserFK = currentUser.Id, GameFK = id, Status = Status });
            }
            else
            {
                entry.Status = Status;
            }

            await _context.SaveChangesAsync();

            // Recalcula o número de jogadas do jogo
            await _context.RecalculateGamePlaysAsync(id);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id, slug });
        }

        /// <summary>
        /// Carrega o jogo (com géneros, plataformas e reviews+autor) e a entrada do utilizador logado.
        /// </summary>
        private async Task<bool> LoadGameAsync(int id, Models.User? currentUser)
        {
            var game = await _context.Games
                .Include(g => g.Genres)
                .Include(g => g.Platforms)
                .Include(g => g.Reviews).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return false;
            }

            Game = game;

            if (currentUser != null)
            {
                MyReview = game.Reviews.FirstOrDefault(r => r.UserFK == currentUser.Id);
                MyEntry = await _context.UserGames.FindAsync(currentUser.Id, id);
            }

            return true;
        }

        /// <summary>
        /// Resolve o User a partir do IdentityUser logado, via email.
        /// </summary>
        private async Task<Models.User?> GetCurrentUserAsync()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return null;
            }

            var idUser = await _userManager.GetUserAsync(User);
            if (idUser?.Email == null)
            {
                return null;
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.Email == idUser.Email);
        }
    }
}
