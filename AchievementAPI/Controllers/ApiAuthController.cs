using AchievementAPI.Dtos;
using AchievementAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AchievementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAuthController : ControllerBase
    {
        // serviço da Microsoft Identity para gerir se um login é bem feito
        private readonly SignInManager<IdentityUser> _signInManager;

        // serviço da Microsoft Identity para gerir users
        private readonly UserManager<IdentityUser> _userManager;

        // serviço que gera chaves de acesso para os users
        private readonly TokenService _tokenService;

        public ApiAuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, TokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Valida o User e a palavra-passe e devolve um token JWT.
        /// </summary>
        /// <param name="dto">Credenciais de login.</param>
        [HttpPost("/api/bearer/login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var identityUser = await _userManager.FindByNameAsync(dto.Username);
            if (identityUser == null)
                return BadRequest("Invalid user or password");

            var resultPassword = await _signInManager.CheckPasswordSignInAsync(identityUser, dto.Password,
                false);

            if (!resultPassword.Succeeded)
                return BadRequest("Invalid user or password");

            // Roles do utilizador → vão para dentro do token para os [Authorize(Roles = "...")] funcionarem.
            var roles = await _userManager.GetRolesAsync(identityUser);
            var token = _tokenService.GenerateToken(identityUser, roles);

            return Ok(token);
        }
    }
}
