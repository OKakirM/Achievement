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

public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public IndexModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    /// <summary>
    /// Nome do utilizador (espelhado em AspNetUsers.UserName e Users.Name).
    /// </summary>
    [BindProperty]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O nome deve ter entre {2} e {1} caracteres.")]
    [Display(Name = "Nome do utilizador")]
    public string Username { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        Username = await _userManager.GetUserNameAsync(user) ?? string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var newName = Username.Trim();
        var currentName = await _userManager.GetUserNameAsync(user);
        if (newName != currentName)
        {
            // AspNetUsers (Identity) — valida unicidade do nome de utilizador.
            var setResult = await _userManager.SetUserNameAsync(user, newName);
            if (!setResult.Succeeded)
            {
                foreach (var error in setResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // Users — tabela personalizada, ligada à Identity pelo e-mail.
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (appUser != null)
            {
                appUser.Name = newName;
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
        }

        StatusMessage = "O teu perfil foi atualizado.";
        return RedirectToPage();
    }
}
