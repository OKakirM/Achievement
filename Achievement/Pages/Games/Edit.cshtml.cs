using Achievement.Data;
using Achievement.Models;
using Achievement.ValidationFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Achievement.Pages.Games
{
    //[Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public EditModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Game Game { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Imagem de Capa")]
        public IFormFile? CoverImageFile { get; set; }
        [BindProperty]
        [Display(Name = "Imagem do Banner")]
        public IFormFile? BannerImageFile { get; set; }

        [BindProperty]
        public bool RemoveCoverImage { get; set; }

        [BindProperty]
        public bool RemoveBannerImage { get; set; }

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
                return RedirectToPagePermanent("./Edit", new { id = game.Id, slug = game.Slug });
            }

            Game = game;

            HttpContext.Session.SetInt32("Game", Game.Id);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var oldId = HttpContext.Session.GetInt32("Game");
            bool hasErrors = false;

            if (!ModelState.IsValid)
            {
                return Page();
            }


            if (Game.Id != oldId)
            {
                return RedirectToPage("./Index");
            }

            // =============================================
            // Validação do tamanho dos campos de texto
            // =============================================

            if (Game.Name.Length > 100)
            {
                ModelState.AddModelError("Game.Name", "O nome do jogo deve ter no máximo 100 caracteres.");
                hasErrors = true;
            }

            if (Game.Description.Length > 2000)
            {
                ModelState.AddModelError("Game.Description", "A descrição do jogo deve ter no máximo 2000 caracteres.");
                hasErrors = true;
            }


            if (Game.ReleaseDate.Year < 1958 || Game.ReleaseDate.Year > 2100)
            {
                ModelState.AddModelError("Game.ReleaseDate", "O ano de lançamento do jogo deve ser 1958 ou posterior.");
                hasErrors = true;
            }

            // =============================================
            // Validação do tamanho do arquivo das imagens
            // - Limite de 10 MB
            // =============================================

            if (CoverImageFile != null && CoverImageFile.Length > CustomValidationFiles._MaxFileSize)
            {
                ModelState.AddModelError("CoverImageFile", "O tamanho do arquivo de imagem de capa excede o limite permitido.");
                hasErrors = true;
            }

            if (BannerImageFile != null && BannerImageFile.Length > CustomValidationFiles._MaxFileSize)
            {
                ModelState.AddModelError("BannerImageFile", "O tamanho do arquivo de imagem do banner excede o limite permitido.");
                hasErrors = true;
            }

            // =============================================
            // Validação do tipo de arquivo das imagens
            // - Apenas JPEG/JPG e PNG
            // =============================================

            if (CoverImageFile != null && CoverImageFile.ContentType != CustomValidationFiles._FileContentTypeJpg
                && CoverImageFile.ContentType != CustomValidationFiles._FileContentTypePng)
            {
                ModelState.AddModelError("CoverImageFile", "O tipo de arquivo de imagem de capa não é suportado. Apenas JPG e PNG são permitidos.");
                hasErrors = true;
            }

            if (BannerImageFile != null && BannerImageFile.ContentType != CustomValidationFiles._FileContentTypeJpg
                && BannerImageFile.ContentType != CustomValidationFiles._FileContentTypePng)
            {
                ModelState.AddModelError("BannerImageFile", "O tipo de arquivo de imagem do banner não é suportado. Apenas JPG e PNG são permitidos.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                return Page();
            }

            // =============================================
            // Salvamento das imagens
            // =============================================

            if (CoverImageFile != null)
            {
                // apagar o ficheiro antigo
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", Game.CoverImage ?? string.Empty);

                if (System.IO.File.Exists(oldFilePath) && Game.CoverImage != CustomValidationFiles._GamesCoverDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                // Geração de um nome único para a imagem
                Guid g1 = Guid.NewGuid();
                string coverImageName = g1.ToString();
                string coverImageExtension = Path.GetExtension(CoverImageFile.FileName).ToLowerInvariant();
                coverImageName += coverImageExtension;

                // Guarda o caminho da imagem no banco de dados
                Game.CoverImage = Path.Combine(CustomValidationFiles._GamesCoverFolder, coverImageName);

                // Caminho físico para salvar a imagem no servidor
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/", CustomValidationFiles._GamesCoverFolder);

                // Verifica se o diretório existe, caso contrário, cria o diretório
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                // Combina o caminho do diretório com o nome do arquivo para obter o caminho completo
                filePath = Path.Combine(filePath, coverImageName);

                // Salva a imagem no servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await CoverImageFile.CopyToAsync(fileStream);
                }

            }
            else if (RemoveCoverImage)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", Game.CoverImage ?? string.Empty);
                if (System.IO.File.Exists(oldFilePath) && Game.CoverImage != CustomValidationFiles._GamesCoverDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                Game.CoverImage = CustomValidationFiles._GamesCoverDefaultImage;
            }

            if (BannerImageFile != null)
            {
                // apagar o ficheiro antigo
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", Game.BannerImage ?? string.Empty);

                if (System.IO.File.Exists(oldFilePath) && Game.BannerImage != CustomValidationFiles._GamesBannerDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                // Geração de um nome único para a imagem
                Guid g2 = Guid.NewGuid();
                string bannerImageName = g2.ToString();
                string bannerImageExtension = Path.GetExtension(BannerImageFile.FileName).ToLowerInvariant();
                bannerImageName += bannerImageExtension;

                // Guarda o caminho da imagem no banco de dados
                Game.BannerImage = Path.Combine(CustomValidationFiles._GamesBannerFolder, bannerImageName);

                // Caminho físico para salvar a imagem no servidor
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/", CustomValidationFiles._GamesBannerFolder);

                // Verifica se o diretório existe, caso contrário, cria o diretório
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                // Combina o caminho do diretório com o nome do arquivo para obter o caminho completo
                filePath = Path.Combine(filePath, bannerImageName);

                // Salva a imagem no servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await BannerImageFile.CopyToAsync(fileStream);
                }
            }
            else if (RemoveBannerImage)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", Game.BannerImage ?? string.Empty);
                if (System.IO.File.Exists(oldFilePath) && Game.BannerImage != CustomValidationFiles._GamesBannerDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                Game.BannerImage = CustomValidationFiles._GamesBannerDefaultImage;
            }

            // Ensure slug
            if (string.IsNullOrWhiteSpace(Game.Slug))
            {
                Game.Slug = Game.Name?.ToLowerInvariant().Replace(' ', '-') ?? string.Empty;
            }

            _context.Attach(Game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(Game.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
