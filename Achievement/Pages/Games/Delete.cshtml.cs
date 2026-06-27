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
    [Authorize(Roles = "Admin")]
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
                .Include(g => g.Platforms)
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
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

                // Só apaga o banner se a imagem não for a default
                if (Game.BannerImage != CustomValidationFiles._GamesBannerDefaultImage)
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
