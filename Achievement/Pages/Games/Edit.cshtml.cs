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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Achievement.Pages.Games
{
    [Authorize(Roles = "Admin")]
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

        // Variável auxiliar para facilitar a digitação da data de lançamento
        [BindProperty]
        [Display(Name = "Data de Lançamento")]
        [RegularExpression(CustomValidationFiles._ReleaseDateRegexPattern, ErrorMessage = "Data inválida. Use yyyy/MM/dd, dd/MM/yyyy ou MM/dd/yyyy.")]
        public string? ReleaseDateInput { get; set; }

        // Popular as listas de seleção para Platformas e gêneros
        public IEnumerable<SelectListItem>? PlatformsList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem>? GenresList { get; set; } = new List<SelectListItem>();

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
            ReleaseDateInput = game.ReleaseDate.ToString("dd/MM/yyyy");

            // Pré-seleciona as plataformas e géneros já associados ao jogo
            SelectedPlatformIds = game.Platforms.Select(p => p.Id).ToArray();
            SelectedGenreIds = game.Genres.Select(g => g.Id).ToArray();

            await LoadSelectedFieldsAsync();

            HttpContext.Session.SetInt32("Game", Game.Id);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var oldId = HttpContext.Session.GetInt32("Game");
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


            if (Game.ReleaseDate.Year < 1958 || Game.ReleaseDate.Year > 2100)
            {
                ModelState.AddModelError("ReleaseDateInput", "O ano de lançamento do jogo deve ser 1958 ou posterior.");
                hasErrors = true;
            }

            // =============================================
            // Validação de Duplicidade dos campos de texto
            // =============================================

            bool duplicatedName = await _context.Games.AnyAsync(g => g.Name == Game.Name && g.Id != Game.Id);

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

            if (hasErrors)
            {
                await LoadSelectedFieldsAsync();
                return Page();
            }

            // =============================================
            // Atualização do jogo
            // =============================================

            // Carrega o jogo existente (com as coleções N-N) para o ALTERAR em vez de criar um novo
            var gameToUpdate = await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == Game.Id);

            if (gameToUpdate == null)
            {
                await LoadSelectedFieldsAsync();
                return NotFound();
            }

            // Atualiza os campos escalares
            gameToUpdate.Name = Game.Name;
            gameToUpdate.Description = Game.Description;
            gameToUpdate.ReleaseDate = Game.ReleaseDate;
            gameToUpdate.Length = Game.Length;

            // =============================================
            // Salvamento das imagens
            // =============================================

            if (CoverImageFile != null)
            {
                // apagar o ficheiro antigo
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", gameToUpdate.CoverImage ?? string.Empty);

                if (System.IO.File.Exists(oldFilePath) && gameToUpdate.CoverImage != CustomValidationFiles._GamesCoverDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                // Geração de um nome único para a imagem
                Guid g1 = Guid.NewGuid();
                string coverImageName = g1.ToString();
                string coverImageExtension = Path.GetExtension(CoverImageFile.FileName).ToLowerInvariant();
                coverImageName += coverImageExtension;

                // Guarda o caminho da imagem no banco de dados
                gameToUpdate.CoverImage = Path.Combine(CustomValidationFiles._GamesCoverFolder, coverImageName);

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
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", gameToUpdate.CoverImage ?? string.Empty);
                if (System.IO.File.Exists(oldFilePath) && gameToUpdate.CoverImage != CustomValidationFiles._GamesCoverDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                gameToUpdate.CoverImage = CustomValidationFiles._GamesCoverDefaultImage;
            }

            if (BannerImageFile != null)
            {
                // apagar o ficheiro antigo
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", gameToUpdate.BannerImage ?? string.Empty);

                if (System.IO.File.Exists(oldFilePath) && gameToUpdate.BannerImage != CustomValidationFiles._GamesBannerDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                // Geração de um nome único para a imagem
                Guid g2 = Guid.NewGuid();
                string bannerImageName = g2.ToString();
                string bannerImageExtension = Path.GetExtension(BannerImageFile.FileName).ToLowerInvariant();
                bannerImageName += bannerImageExtension;

                // Guarda o caminho da imagem no banco de dados
                gameToUpdate.BannerImage = Path.Combine(CustomValidationFiles._GamesBannerFolder, bannerImageName);

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
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", gameToUpdate.BannerImage ?? string.Empty);
                if (System.IO.File.Exists(oldFilePath) && gameToUpdate.BannerImage != CustomValidationFiles._GamesBannerDefaultImage)
                    System.IO.File.Delete(oldFilePath);

                gameToUpdate.BannerImage = CustomValidationFiles._GamesBannerDefaultImage;
            }

            // =============================================
            // Associação de Plataformas e Géneros (N-N)
            // - Limpa as associações atuais e re-adiciona as selecionadas
            // =============================================

            gameToUpdate.Platforms.Clear();
            gameToUpdate.Genres.Clear();

            // Se houver plataformas selecionadas, busca as entidades correspondentes e associa ao jogo
            if (SelectedPlatformIds.Length > 0)
            {
                var platforms = await _context.Platforms
                    .Where(p => SelectedPlatformIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var platform in platforms)
                    gameToUpdate.Platforms.Add(platform);
            }

            // Se houver gêneros selecionados, busca as entidades correspondentes e associa ao jogo
            if (SelectedGenreIds.Length > 0)
            {
                var genres = await _context.Genres
                    .Where(g => SelectedGenreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in genres)
                    gameToUpdate.Genres.Add(genre);
            }

            // Gera um slug único para o jogo com base no nome atualizado
            gameToUpdate.Slug = await GenerateUniqueSlugAsync(GenerateSlug(gameToUpdate.Name), gameToUpdate.Id);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(gameToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Games/Details", new { id = gameToUpdate.Id, slug = gameToUpdate.Slug });
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
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
        private async Task<string> GenerateUniqueSlugAsync(string baseSlug, int excludeId)
        {
            var slug = baseSlug;
            var counter = 1;

            while (await _context.Games.AnyAsync(g => g.Slug == slug && g.Id != excludeId))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}
