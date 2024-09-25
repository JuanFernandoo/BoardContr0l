using BoardContr0l.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BoardContr0l.Models;
using Microsoft.EntityFrameworkCore;
using System;
using BoardContr0l.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoardContr0l.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignUpLogin : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public SignUpLogin(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService; // Logica de registro
            _context = context; // Conector a base de datos
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username); // Verifica si el nombre de usuario ya existe
            if (existingUser != null)
            {
                return BadRequest("El nombre de usuario esta en uso"); 
            }

            var role = await _context.Roles.FindAsync(request.RoleId); // Verifica que el id sea valido
            if (role == null)
            {
                return BadRequest("El id no existe"); //Devuelve error 400 si el no existe
            }

            var user = new User // Creación del nuevo usuario
            {
                Username = request.Username,
                Email = request.Email,
                RoleId = role.RoleId,
                Status = true,
                CreateBy = Environment.MachineName, 
                ModifyBy = Environment.MachineName, 
                CreateDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow 
            };

            user.PasswordHash = HashPassword(request.Password); 

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(); // Guarda

            return Ok(new // Retorna codigo 200 y las propiedades
            {
                userId = user.UserId,
                username = user.Username,
                email = user.Email,
                roleId = user.RoleId,
                status = user.Status
            });
        }
        private string HashPassword(string password) //Convierte la contraseña en texto plano
        {
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.HashPassword(null, password);
        }


        private bool VerifyPassword(string plainPassword, string hashedPassword) // Compara el hash con la contraseña ingresasa
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }

    public class RegisterRequest // Define la estructura del login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; } 
    }


}
