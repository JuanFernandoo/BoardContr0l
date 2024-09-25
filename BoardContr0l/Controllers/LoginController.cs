using BoardContr0l.Data;
using BoardContr0l.Models;
using BoardContr0l.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoardContr0l.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public LoginController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService)); // logica del usuario
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Acceso a datos 
        }

        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest("Ingrese los datos requeridos"); // Verifica que los esapcios no esten en blanco, error 400
            }

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password cannot be empty.");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username); // Busca la veracidad del usuario
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash)) // Busca la veracidad de la contraseña
            {
                return Unauthorized("Usuario y/o contraseña incorrecta"); // Respuesta si la contraseña y el usuario no esta en la base de datos
            }

            // Generar el token JWT
            var token = GenerateJwtToken(user); // Generación de token par la autorización

            return Ok(new // Retorno con doigo 200 exitoso
            {
                userId = user.UserId,
                username = user.Username,
                email = user.Email,
                roleId = user.RoleId,
                status = user.Status,
                token 
            });

        }
        private string GenerateJwtToken(User user)
        {
            var roleName = GetRoleName(user.RoleId);
            var claims = new[]
            {

        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, roleName)
    };

            // Clave de seguridad del token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("B04rd_C0ntr01_FNZ_2024_B04rd_C0ntr01_FNZ_2024"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7132",
                audience: "https://localhost:7132",
                expires: DateTime.Now.AddMinutes(30),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // retorno del token
        }
        private string GetRoleName(int roleId) // Toma el id del rol y lo devuelve en con su rol correspondiente
        {
            switch (roleId)
            {
                case 1: return "Admin";
                case 2: return "User";
                default: return "User"; // Rol por defecto
            }
        }
            private bool VerifyPassword(string plainPassword, string hashedPassword) // Verifica las contraseña con el texto plano de la base de datos
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }

        public class LoginRequest // Define la estrucutra del login 
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
