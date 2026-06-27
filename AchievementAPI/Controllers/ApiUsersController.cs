using Achievement.Data;
using Achievement.Models;
using AchievementAPI.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchievementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApiUsersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Lista todos os utilizadores.
        /// </summary>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            // Obtém todos os utilizadores da tabela Users, sem rastrear alterações.
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return Ok(users.Select(u => MapToDto(u)));
        }

        /// <summary>
        /// Obtém um utilizador pelo Id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            // Obtém o utilizador pelo Id.
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(MapToDto(user));
        }

        /// <summary>
        /// Cria um utilizador, criando também a conta de login na AspNetUsers (Identity).
        /// Falha se o e-mail já estiver registado.
        /// </summary>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<UserDto>> PostUser(UserCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _context.Users.AnyAsync(x => x.Email == dto.Email);
            if (exists) return BadRequest("User with this email already exists.");

            // Cria a conta de login na AspNetUsers com o e-mail e password fornecidos.
            var idUser = new IdentityUser { UserName = dto.Name, Email = dto.Email, EmailConfirmed = true };
            var created = await _userManager.CreateAsync(idUser, dto.Password);
            if (!created.Succeeded)
            {
                foreach (var e in created.Errors) ModelState.AddModelError(string.Empty, e.Description);
                return BadRequest(ModelState);
            }

            // Cria o utilizador na tabela Users com os dados fornecidos.
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Image = dto.Image,
                Banner = dto.Banner,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, MapToDto(user));
        }

        /// <summary>
        /// Atualiza nome, e-mail e imagem de um utilizador, refletindo o nome/e-mail na AspNetUsers (Identity).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> PutUser(int id, UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Atualiza a conta de login espelhada (ligada por e-mail) na AspNetUsers.
            var idUser = await _userManager.FindByEmailAsync(user.Email);
            if (idUser != null)
            {
                idUser.UserName = dto.Name;
                idUser.Email = dto.Email;
                idUser.EmailConfirmed = true;
                var upd = await _userManager.UpdateAsync(idUser);
                if (!upd.Succeeded)
                {
                    foreach (var e in upd.Errors) ModelState.AddModelError(string.Empty, e.Description);
                    return BadRequest(ModelState);
                }
            }

            // Atualiza o utilizador na tabela Users com os dados fornecidos.
            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Image = dto.Image;
            user.Banner = dto.Banner;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Remove um utilizador, eliminando também a conta de login na AspNetUsers (Identity).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Remove a conta de login espelhada (ligada por e-mail) na AspNetUsers.
            var idUser = await _userManager.FindByEmailAsync(user.Email);
            if (idUser != null) await _userManager.DeleteAsync(idUser);

            // Remove o utilizador na tabela Users.
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static UserDto MapToDto(User u) => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Image = u.Image };
    }
}
