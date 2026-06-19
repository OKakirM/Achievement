using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Authorization;
using Achievement.ValidationFiles;

namespace Achievement.Pages.Games
{
    //[Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public DeleteModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Game Game { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, string? slug)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Genres)
                .Include(g => g.Plataforms)
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            // Se o slug na URL não corresponder ao slug real do jogo,
            // redireciona para a URL correta.
            if (string.IsNullOrEmpty(slug) || slug != game.Slug)
            {
                return RedirectToPagePermanent("./Delete", new { id = game.Id, slug = game.Slug });
            }

            Game = game;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                Game = game;

                // Só apaga o cover se a imagem não for a default
                if(Game.CoverImage != CustomValidationFiles._GamesCoverDefaultImage)
                {
                    // Apagar a imagem associada
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", Game.CoverImage ?? string.Empty);

                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                // Só apaga o bammer se a imagem não for a default
                if (Game.CoverImage != CustomValidationFiles._GamesCoverDefaultImage)
                {
                    // Apagar a imagem associada
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", Game.BannerImage ?? string.Empty);

                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
