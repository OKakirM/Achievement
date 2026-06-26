// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Achievement.Data;

namespace Achievement.Areas.Identity.Pages.Account.Manage;

public class EmailModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public EmailModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public string? Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [Display(Name = "Novo email")]
        public string NewEmail { get; set; } = default!;
    }

    private async Task LoadAsync(IdentityUser user)
    {
        var email = await _userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email!,
        };

        IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var newEmail = Input.NewEmail.Trim();
        var oldEmail = user.Email;
        if (string.Equals(newEmail, oldEmail, System.StringComparison.OrdinalIgnoreCase))
        {
            StatusMessage = "O teu email não foi alterado.";
            return RedirectToPage();
        }

        // Unicidade do email entre as duas tabelas.
        var existingIdUser = await _userManager.FindByEmailAsync(newEmail);
        var emailTakenInUsers = await _context.Users.AnyAsync(u => u.Email == newEmail && u.Email != oldEmail);
        if ((existingIdUser != null && existingIdUser.Id != user.Id) || emailTakenInUsers)
        {
            ModelState.AddModelError("Input.NewEmail", "Este email já está em uso.");
            await LoadAsync(user);
            return Page();
        }

        // AspNetUsers (Identity). Sem email sender, confirma-se diretamente.
        var setEmailResult = await _userManager.SetEmailAsync(user, newEmail);
        if (!setEmailResult.Succeeded)
        {
            foreach (var error in setEmailResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            await LoadAsync(user);
            return Page();
        }
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        // Users — tabela personalizada, localizada pelo email antigo (a ligação).
        var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == oldEmail);
        if (appUser != null)
        {
            appUser.Email = newEmail;
            await _context.SaveChangesAsync();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "O teu email foi atualizado.";
        return RedirectToPage();
    }
}
