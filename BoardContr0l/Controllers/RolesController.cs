using BoardContr0l.Data;
using BoardContr0l.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BoardContr0l.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context; // conector a la base de datos
        }

        [HttpPost("create-role")] // Creación de roles
        [Authorize(Roles = "Admin")] //Solo los administradore pueden crear

        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var existingRole = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == request.RoleName); // Verifica que el ROL exista
            if (existingRole != null)
            {
                return BadRequest("Rol ya existente"); // Si el rol existe,  devuelve error 400
            }

            var role = new Role
            {
                // Crea el rol y sus propiedades correspondientes
                RoleName = request.RoleName,
                Status = request.Status,
                CreateBy = Environment.MachineName,
                ModifyBy = Environment.MachineName, 
                CreateDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync(); // Guarda

            return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleId }, role); // Retorna role de ID creado
        }

        [HttpPut("update-rol")] // Actualiza rol
        [Authorize(Roles = "Admin")] //Solo los administradore pueden actualizar 
        public IActionResult UpdateRole(int id, [FromBody] Role role)
        {
            if (role == null || id != role.RoleId)
            {
                return BadRequest("El rol no existe"); // Verifica que el objeto que se ingrese coincida con el id,  sino lo encuentra devuelve error 400
            }

            var existingRole = _context.Roles.Find(id); 
            if (existingRole == null)
            {
                return NotFound(); // Verifica que el id del rol ingresedao coincida en la base de datos, sino error 400
            }

            existingRole.RoleName = role.RoleName;
            existingRole.Status = role.Status;
            existingRole.ModifyBy = Environment.MachineName;
            existingRole.ModifiedDate = DateTime.UtcNow;

            _context.Roles.Update(existingRole);
            _context.SaveChanges(); // Guarda

            return Ok(new { message = "El rol se ha actualizado", role = existingRole  }); // Respuesta 200 el tablero se ha actuaizado
        }

        [HttpGet("full-roles")] // Visualización de todos los roles
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros
        public async Task<IActionResult> GetRole()
        {
            var roles = await _context.Roles // Incluye en la respuesta propiedades de roles
                .Select(x => new
                {
                    x.RoleId,
                    x.RoleName,
                    x.Status
                })
                .ToListAsync();

            return Ok(roles); // Devuelve respuesta 200 con las propiedades de roles
        }

        [HttpGet("id-roles-list")] // Visualiza roles por id 
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros por id
        public async Task<IActionResult> GetRoleById(int id) 
        {
            var role = await _context.Roles.FindAsync(id); // verifica la existencia del id 
            if (role == null)
            {
                return NotFound(); // error 400 si el rol no existe
            }

            return Ok(role); // Devuelve el role con el id ingresado
        }

        [HttpGet("collated-roles")] // Lista de paginas
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

            int totalRecords = await _context.Roles.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

            if (page > totalPages)
            {
                return BadRequest("El número de página excede el total de páginas disponibles."); // Calcula el total de registros y el numero de paginas
            }

            var roles = await _context.Roles // Devuelve segun la pagina soliicitada
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return Ok(new // Respuesta 200 con la info solicitada
            {
                currentPage = page,
                totalPages = totalPages,
                pagesize = pagesize,
                totalRecords = totalRecords,
                data = roles
            });
        }


        [HttpDelete("delete-role-id")] // Elimina rol por id 
        [Authorize(Roles = "Admin")] // Solo el admin tiene el permiso para eliminar 
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(x => x.RoleId == roleId);
            if (role == null)
            {
                return NotFound("Role not found"); // Si no encuentra el rol por el id, devuelve el error 
            }

            _context.Roles.Remove(role); // Elimina
            await _context.SaveChangesAsync(); // Guarda
            return Ok(new { message = "Eliminación exitosa " }); // Respuesta de elemininación existosa
        }

        public class CreateRoleRequest // Define la estructura de la creación de un rol
        {
            public string RoleName { get; set; }
            public bool Status { get; set; }
        }

    }
}
