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
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly Achievement.Data.ApplicationDbContext _context;

        public CreateModel(Achievement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            await LoadSelectedFieldsAsync();
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

        // Variável auxiliar para facilitar a digitação da data de lançamento
        [BindProperty]
        [Display(Name = "Data de Lançamento")]
        [RegularExpression(CustomValidationFiles._ReleaseDateRegexPattern, ErrorMessage = "Data inválida. Use yyyy/MM/dd, dd/MM/yyyy ou MM/dd/yyyy.")]
        public string? ReleaseDateInput { get; set; }

        // Popular as listas de seleção para Platformas e gêneros
        public IEnumerable<SelectListItem> PlatformsList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> GenresList { get; set; } = new List<SelectListItem>();

        // Variavel para armazenar as Platformas e gêneros selecionados
        /// <summary>
        /// Armazena os IDs das plataformas selecionadas pelo usuário no formulário de criação do jogo.
        /// </summary>
        [BindProperty]
        public int[] SelectedPlatformIds { get; set; } = Array.Empty<int>();
        
        /// <summary>
        /// Armazena os IDs dos gêneros selecionados pelo usuário no formulário de criação do jogo.
        /// </summary>
        [BindProperty]
        public int[] SelectedGenreIds { get; set; } = Array.Empty<int>();

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            bool hasErrors = false;

            // Converte a data para DateTime antes de validar o modelo
            if (CustomValidationFiles.TryParseReleaseDate(ReleaseDateInput, out var releaseDate))
            {
                Game.ReleaseDate = releaseDate;
                ModelState.Remove("Game.ReleaseDate");
            }
            else
            {
                ModelState.AddModelError("ReleaseDateInput", "Data inválida. Use yyyy/MM/dd, dd/MM/yyyy ou MM/dd/yyyy.");
            }

            // Verifica se o modelo é válido antes de tentar salvar
            if (!ModelState.IsValid)
            {
                await LoadSelectedFieldsAsync();
                return Page();
            }

            // =============================================
            // Validação do tamanho dos campos de texto
            // =============================================

            if (Game.Name.Length > 100 || Game.Name.Length < 2)
            {
                ModelState.AddModelError("Game.Name", "O nome do jogo deve ter entre 2 e 100 caracteres.");
                hasErrors = true;
            }

            if (Game.Description.Length > 2000 || Game.Description.Length < 10)
            {
                ModelState.AddModelError("Game.Description", "A descrição do jogo deve ter entre 10 e 2000 caracteres.");
                hasErrors = true;
            }


            if(Game.ReleaseDate.Year < 1958 || Game.ReleaseDate.Year > 2100)
            {
                ModelState.AddModelError("ReleaseDateInput", "O ano de lançamento do jogo deve ser 1958 ou posterior.");
                hasErrors = true;
            }

            // Rating funciona com a média das reviews dos utilizadores
            Game.Rating = 0;

            // =============================================
            // Validação de Duplicidade dos campos de texto
            // =============================================

            bool duplicatedName = await _context.Games.AnyAsync(g => g.Name == Game.Name);

            if (duplicatedName)
            {
                ModelState.AddModelError("Game.Name", "Já existe um jogo com este nome. Por favor, escolha outro nome.");
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
                await LoadSelectedFieldsAsync();
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

            // =============================================
            // Associação de Plataformas e Géneros (N-N)
            // =============================================

            if (SelectedPlatformIds.Length > 0)
            {
                var platforms = await _context.Platforms
                    .Where(p => SelectedPlatformIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var platform in platforms)
                    Game.Platforms.Add(platform);
            }

            if (SelectedGenreIds.Length > 0)
            {
                var genres = await _context.Genres
                    .Where(g => SelectedGenreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in genres)
                    Game.Genres.Add(genre);
            }

            Game.Slug = await GenerateUniqueSlugAsync(GenerateSlug(Game.Name));
            Game.CreatedAt = DateTime.UtcNow;

            _context.Games.Add(Game);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Games/Details", new { id = Game.Id, slug = Game.Slug });
        }

        /// <summary>
        /// Preenche os fields necessários de chaves estrangeiras, exclusivamente para plataformas e gêneros da base de dados
        /// </summary>
        /// <returns></returns>
        private async Task LoadSelectedFieldsAsync()
        {
            PlatformsList = await _context.Platforms
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToListAsync();

            GenresList = await _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                .ToListAsync();
        }

        /// <summary>
        /// Gera um slug a partir do nome do jogo, removendo caracteres especiais e espaços, e substituindo-os por hífens.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GenerateSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "jogo";
            var s = value.Trim().ToLowerInvariant();
            s = Regex.Replace(s, "[^a-z0-9]+", "-");
            s = s.Trim('-');
            return string.IsNullOrWhiteSpace(s) ? "jogo" : s;
        }

        /// <summary>
        /// Gera um slug único verificando se já existe na base de dados. Se existir, adiciona um número incremental ao final do slug.
        /// </summary>
        /// <param name="baseSlug"></param>
        /// <returns></returns>
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
