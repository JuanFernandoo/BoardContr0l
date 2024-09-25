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
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public UsersController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService)); // Logica de usuarios
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Conector a base de datos
        }

        [HttpGet("full-users")] // Visualiza todos los usuarios
        [Authorize(Roles = "Admin")] // Solo los usuarios con admin pueden visualizar 
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role) // Asocia la tabla de roles 
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.RoleId,
                    u.Status,
                    RoleName = u.Role.RoleName 
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("User-Id")] // Visualizar usuarios por ID 
        [Authorize(Roles = "Admin")] // Solo los usuarios con admin pueden visualizar 
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role) // Asocia con tabla roles
                .Where(u => u.UserId == id) 
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.RoleId,
                    u.Status,
                    RoleName = u.Role.RoleName 
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("El ID ingresado NO existe"); // Error 400  si el rol ingresado NO existe 
            }

            return Ok(user); // Devuelve el usuario encontrado 
        }

        [HttpGet("collated")] // Lista de paginas
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar a gusto 
        public async Task<IActionResult> GetPaginatedBoards([FromQuery] int page = 1, [FromQuery] int pagesize = 8)
        {

            if (page <= 0)
            {
                return BadRequest("El número de página debe ser mayor que 0."); // Verifica que el tamaño que ingrese de paginas sea valido
            }

            if (pagesize <= 0)
            {
                return BadRequest("El tamaño de página debe ser mayor que 0"); // verifica que el tamaño de las pagians sea valido
            }

            int totalRecords = await _context.Users.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

            if (page > totalPages)
            {
                return BadRequest("El número de página excede el total de páginas disponibles."); // Calcula el total de registros y el numero de paginas
            }

            var users = await _context.Users // Devuelve segun la pagina soliicitada
                .Include(u => u.Role)
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return Ok(new // Respuesta 200 con la info solicitada
            {
                currentPage = page,
                totalPages = totalPages,
                pagesize = pagesize,
                totalRecords = totalRecords,
                data = users
            });
        }

        [HttpPut("update-user")] // Actualiza 
        [Authorize(Roles = "Admin")] // Solo el admin tiene el permiso para actualizar 
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UpdateUserRequest request)
        {
            if (request == null)
            {
                return BadRequest("Ingrese los datos correspondientes"); // Error 400 si el espacio esta en blanco
            }

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                return NotFound("Usuario no encontrado"); // SI el usuario ingresado no se encuentra en la base de datos error 400
            }

            user.Username = request.Username ?? user.Username; 
            user.Email = request.Email ?? user.Email;
            user.Status = request.Status;

            user.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();  // Guarda

            return Ok(new // Respuesta con code 200 y usuario actualizado
            {
                message = "Usuario actualizado",
                userId = user.UserId,
                username = user.Username,
                email = user.Email,
                status = user.Status
            });
        }

        [HttpDelete("Delete-by-username")] // Elimina por username 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByUsername(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username); // Conecta con la base de datos 
            if (user == null)
            {
                return NotFound("Usuario no encontrado"); // Si el usuario no esta en la base de datos, permite eliminarlo
            }

            _context.Users.Remove(user); // Elimina
            await _context.SaveChangesAsync(); // Guarda
            return NoContent();
        }


        public class UpdateUserRequest // Estructura de actualización
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public bool Status { get; set; }
        }
    }
}
