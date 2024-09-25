using BoardContr0l.Data;
using BoardContr0l.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoardContr0l.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AuthController (ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost ("Login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Users.SingleOrDefault (u=> u.Username == login.Username);
            if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
            {
                return Unauthorized ("Crednciales incorrectas");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Sub, user.Username),
                new Claim ("userId", user.UserId.ToString()),
                new Claim ("roleId", user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("B04rd_C0ntr01_FNZ_2024"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7132",
                audience: "https://localhost:7132",
                claims: claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword (string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }

}
