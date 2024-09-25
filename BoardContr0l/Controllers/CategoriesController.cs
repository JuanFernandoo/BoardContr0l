using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BoardContr0l.Data;
using BoardContr0l.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BoardContr0l.Controllers
{
    [Route("api/categories")] 
    [ApiController]
    [Authorize] // Restringe el acceso a usuarios no autenticados
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-category")] // Crear una categoria 
        [Authorize(Roles = "Admin")]  // Solo los usuarios con admin pueden crear
        public IActionResult CreateCategory([FromBody] Category category) // Relación a tabla de Category y conexión de sus propiedades
        {
            if (category == null)
            {
                return BadRequest(); // Si el board es nulo, devulve un error 400 
            }

            // Valores que se ingresaran automatizados 
            category.CreatedBy = Environment.MachineName;
            category.CreatedDate = DateTime.UtcNow;
            category.ModifiedBy = null; 
            category.ModifiedDate = null;

            _context.Categories.Add(category); // Agrega os datos ingresados a ala base de datos
            _context.SaveChanges(); // Guarda cambios

            return CreatedAtAction(nameof(GetCategoryDetails), new { id = category.CategoryId }, category); // Retorna respuesta 201 con detalles del tablero que se creo
        }

        [HttpPut("update-category")] // Actualiza categoria 
        [Authorize(Roles = "Admin")] // Solo los usuarios con admin pueden actualizar
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            if (category == null || id != category.CategoryId) 
            {
                return BadRequest(); // Si la categoria o el id no coinciden devuelve error  400
            }

            var existingCategory = _context.Categories.Find(id); // Verifica la existencia de la categoria en la base de datos 
            if (existingCategory == null)
            {
                return NotFound(); // Si el tablero no existe devueve error 400
            }

            // Valores que se ingresaran automatizados 
            existingCategory.Status = category.Status;
            existingCategory.Description = category.Description;
            existingCategory.ModifiedBy = Environment.MachineName; 
            existingCategory.ModifiedDate = DateTime.UtcNow;

            _context.Categories.Update(existingCategory); // Actualiza el tablero con los datos que se ingresaron
            _context.SaveChanges(); // Guarda cambios

            return Ok(new { message = "La categoria se ha actualizado", board = existingCategory }); // Respuesta 200 el tablero se ha actuaizado
        }

        [HttpGet("full-categories")]  // Visualiza todos los boards ingresados
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros por id
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList(); 
            if (!categories.Any()) 
            {
                return NotFound(); //  Si no encuentra categorias devuelve error 400 
            }

            return Ok(categories); // Devuelve categorias
        }

        [HttpGet("list-category-id")] // Visualiza tableros por id 
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros por id
        public IActionResult GetCategoryDetails(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) // Verifica la existencia del id 
            {
                return NotFound(); // error 400 si no encuentra el id
            }

            return Ok(category); // Devuelve respuesta 200 con el objetoco que se crea para la respuesta
        }

        [HttpGet("collated-categories")] // Lista de paginas
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

            int totalRecords = await _context.Boards.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

            if (page > totalPages)
            {
                return BadRequest("El número de página excede el total de páginas disponibles."); // Calcula el total de registros y el numero de paginas
            }

            var boards = await _context.Boards // Devuelve segun la pagina soliicitada
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return Ok(new // Respuesta 200 con la info solicitada
            {
                currentPage = page,
                totalPages = totalPages,
                pagesize = pagesize,
                totalRecords = totalRecords,
                data = boards
            });
        }

        [HttpDelete("delete-categories-id")] // Elimina categories  por id 
        [Authorize(Roles = "Admin")] // Solo el admin tiene el permiso para eliminar 
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);  
            if (category == null) // Si no encuentra la categoria por el id, devuelve el error 
            {
                return NotFound();
            }

            _context.Categories.Remove(category); // elimina
            _context.SaveChanges(); // Guarda

            return Ok(new { message = "Eliminación exitosa " }); // Respuesta de elemininación existosa
        }
    }
}
