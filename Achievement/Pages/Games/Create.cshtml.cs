using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Achievement.ValidationFiles;
using System.ComponentModel.DataAnnotations;

namespace Achievement.Pages.Games
{
    //[Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public CreateModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Game Game { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Imagem de Capa")]
        public IFormFile? CoverImageFile { get; set; }
        [BindProperty]
        [Display(Name = "Imagem do Banner")]
        public IFormFile? BannerImageFile { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            bool hasErrors = false;
            try
            {
                // Verifica se o modelo é válido antes de tentar salvar
                if (!ModelState.IsValid)
                {
                    return Page();
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


                if(Game.ReleaseDate.Year < 1958 || Game.ReleaseDate.Year > 2100)
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

                if(hasErrors)
                {
                    return Page();
                }

                // =============================================
                // Salvamento das imagens
                // =============================================

                if (CoverImageFile != null)
                {
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
                else
                {
                    // Atribuição da imagem padrão caso nenhuma imagem seja enviada
                    Game.CoverImage = CustomValidationFiles._GamesCoverDefaultImage;
                }

                if (BannerImageFile != null)
                {
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
                else
                {
                    // Atribuição da imagem padrão caso nenhuma imagem seja enviada
                    Game.BannerImage = CustomValidationFiles._GamesBannerDefaultImage;
                }

                Game.Slug = await GenerateUniqueSlugAsync(GenerateSlug(Game.Name));

                _context.Games.Add(Game);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Details/" + Game.Id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Erro ao processar o pedido, por favor tente mais tarde");
                return Page();
            }
        }

        private static string GenerateSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "jogo";
            var s = value.Trim().ToLowerInvariant();
            s = Regex.Replace(s, "[^a-z0-9]+", "-");
            s = s.Trim('-');
            return string.IsNullOrWhiteSpace(s) ? "jogo" : s;
        }

        private async Task<string> GenerateUniqueSlugAsync(string baseSlug)
        {
            var slug = baseSlug;
            var counter = 1;

            while (await _context.Games.AnyAsync(g => g.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}
