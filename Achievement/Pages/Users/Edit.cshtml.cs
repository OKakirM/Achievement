using Achievement.Data;
using Achievement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Achievement.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private const string AdminRole = "Admin";

        private readonly Achievement.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(Achievement.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        /// <summary>
        /// Nova palavra-passe (opcional). Se preenchida, substitui a atual; caso contrário, mantém-se.
        /// </summary>
        [BindProperty]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Palavra-Passe")]
        public string? NewPassword { get; set; }

        [BindProperty]
        public bool IsAdmin { get; set; }

        [BindProperty]
        public bool RemoveImage { get; set; }

        [BindProperty]
        public bool RemoveBanner { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            User = user;

            var idUser = await _userManager.FindByEmailAsync(user.Email);
            IsAdmin = idUser != null && await _userManager.IsInRoleAsync(idUser, AdminRole);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // A password vive na propriedade opcional NewPassword; os campos [Required] do
            // modelo User não estão no formulário, por isso ignoram-se na validação.
            ModelState.Remove("User.Password");
            ModelState.Remove("User.ConfirmPassword");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // E-mail único entre os utilizadores
            if (await _context.Users.AnyAsync(u => u.Email == User.Email && u.Id != User.Id))
            {
                ModelState.AddModelError("User.Email", "E-mail já em uso por outro utilizador.");
                return Page();
            }

            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var newName = User.Name?.Trim() ?? existing.Name;
            var newEmail = User.Email?.Trim() ?? existing.Email;

            // A tabela User está espelhada na AspNetUsers (Identity), ligadas pelo e-mail.
            // Qualquer alteração a nome/e-mail/password/role tem de ser refletida lá também.
            var idUser = await _userManager.FindByEmailAsync(existing.Email);
            if (idUser != null)
            {
                if (!string.IsNullOrWhiteSpace(NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(idUser);
                    var pwdResult = await _userManager.ResetPasswordAsync(idUser, token, NewPassword);
                    if (!pwdResult.Succeeded)
                    {
                        foreach (var e in pwdResult.Errors)
                            ModelState.AddModelError("NewPassword", e.Description);
                        return Page();
                    }
                }

                idUser.UserName = newName;
                idUser.Email = newEmail;
                idUser.EmailConfirmed = true; // admin a alterar: mantém a conta confirmada
                var updateResult = await _userManager.UpdateAsync(idUser);
                if (!updateResult.Succeeded)
                {
                    foreach (var e in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return Page();
                }

                // Sincroniza o papel de Admin com a checkbox
                var alreadyAdmin = await _userManager.IsInRoleAsync(idUser, AdminRole);
                if (IsAdmin && !alreadyAdmin)
                {
                    await _userManager.AddToRoleAsync(idUser, AdminRole);
                }
                else if (!IsAdmin && alreadyAdmin)
                {
                    // ponytail: não impede a auto-despromoção do último admin; admin pode repor pela BD se enganar.
                    await _userManager.RemoveFromRoleAsync(idUser, AdminRole);
                }
            }

            // Atualiza a tabela personalizada
            existing.Name = newName;
            existing.Email = newEmail;

            existing.Image = ResolveImage(existing.Image, User.Image, RemoveImage);
            existing.Banner = ResolveImage(existing.Banner, User.Banner, RemoveBanner);

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                var hasher = new PasswordHasher<User>();
                existing.Password = hasher.HashPassword(existing, NewPassword);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("./Index");
        }

        // Decide o caminho final da imagem: remover (apaga ficheiro) tem prioridade sobre o valor do form.
        private static string? ResolveImage(string? current, string? posted, bool remove)
        {
            if (remove)
            {
                DeletePhysicalFile(current);
                return null;
            }
            return string.IsNullOrWhiteSpace(posted) ? current : posted.Trim();
        }

        private static void DeletePhysicalFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var full = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
            if (System.IO.File.Exists(full))
            {
                System.IO.File.Delete(full);
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
