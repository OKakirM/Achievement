using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Achievement.Data;
using Achievement.Models;
using Achievement.ValidationFiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Achievement.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public User ProfileUser { get; set; } = default!;

        // O perfil pertence ao utilizador logado? (só o dono pode trocar as imagens)
        public bool IsOwner { get; private set; }

        [BindProperty]
        public IFormFile? AvatarFile { get; set; }
        [BindProperty]
        public IFormFile? BannerFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            return await LoadAsync(id.Value) ? Page() : NotFound();
        }

        public async Task<IActionResult> OnPostAvatarAsync(int id)
        {
            if (!await LoadAsync(id)) return NotFound();
            if (!IsOwner) return Forbid();
            if (!ValidImage(AvatarFile, "AvatarFile")) return Page();

            ProfileUser.Image = await SaveImageAsync(AvatarFile!, CustomValidationFiles._UsersAvatarFolder);
            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostBannerAsync(int id)
        {
            if (!await LoadAsync(id)) return NotFound();
            if (!IsOwner) return Forbid();
            if (!ValidImage(BannerFile, "BannerFile")) return Page();

            ProfileUser.Banner = await SaveImageAsync(BannerFile!, CustomValidationFiles._UsersBannerFolder);
            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }

        /// <summary>
        /// Carrega o utilizador (com os jogos da sua lista) e determina se é o dono do perfil.
        /// </summary>
        private async Task<bool> LoadAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserGames).ThenInclude(ug => ug.Game)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return false;
            ProfileUser = user;

            if (base.User.Identity?.IsAuthenticated == true)
            {
                var idUser = await _userManager.GetUserAsync(base.User);
                IsOwner = idUser?.Email != null &&
                          string.Equals(idUser.Email, ProfileUser.Email, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }

        private bool ValidImage(IFormFile? file, string key)
        {
            if (file == null)
            {
                ModelState.AddModelError(key, "Seleciona um ficheiro.");
                return false;
            }
            if (file.Length > CustomValidationFiles._MaxFileSize)
            {
                ModelState.AddModelError(key, "O ficheiro excede o limite de 10 MB.");
                return false;
            }
            if (file.ContentType != CustomValidationFiles._FileContentTypeJpg &&
                file.ContentType != CustomValidationFiles._FileContentTypePng)
            {
                ModelState.AddModelError(key, "Apenas JPG e PNG são permitidos.");
                return false;
            }
            return true;
        }

        private static async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            var name = Guid.NewGuid() + Path.GetExtension(file.FileName).ToLowerInvariant();
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);
            Directory.CreateDirectory(dir);

            using (var fs = new FileStream(Path.Combine(dir, name), FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return $"{folder}/{name}";
        }

        /// <summary>
        /// Nome de apresentação ([Display(Name=...)]) de um estado da lista.
        /// </summary>
        public static string StatusName(GameStatus status)
        {
            var member = typeof(GameStatus).GetMember(status.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? status.ToString();
        }
    }
}
