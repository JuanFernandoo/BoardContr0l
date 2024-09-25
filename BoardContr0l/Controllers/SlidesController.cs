using BoardContr0l.Data;
using BoardContr0l.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardContr0l.Dto;
using Microsoft.AspNetCore.Authorization;

namespace BoardContr0l.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Restringe el acceso a usuarios no autenticados
    public class SlidesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SlidesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-slide")] // Crea slide
        [Authorize(Roles = "Admin")] // Solo los usuarios con admin pueden crear
        public async Task<IActionResult> CreateSlide([FromBody] Slide slide)  // Relación a tabla de Board y conexión de sus propiedades
        {
            if (slide == null)
            {
                return BadRequest("Ingrese los datos correspondientes"); // Error 400 si esta vacio algun dato 
            }

            var boardExists = await _context.Boards.AnyAsync(b => b.BoardId == slide.BoardId); // Verifica la existencia del Id
            if (!boardExists)
            {
                return NotFound($"El Board con ID {slide.BoardId} no existe."); // Si el id no existe, devuelve error 400
            }

            slide.CreatedBy = Environment.MachineName;
            slide.CreatedDate = DateTime.UtcNow;
            slide.ModifiedBy = null; 
            slide.ModifiedDate = null;

            try  // Verifica excepciones
            { 
                _context.Slides.Add(slide);
                await _context.SaveChangesAsync(); // Guarda
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error al guardar en la base de datos: {ex.Message}"); // Retorna error si no permite crear la slide conectada a la base de datos
            }

            return CreatedAtAction(nameof(GetSlideDetails), new { id = slide.SlideId }, slide); // Respuesta d exito y con detalle de los propiedades de board
        }


        [HttpPut("update-slide")] // Actualiza slides 
        [Authorize(Roles = "Admin")] // Solo los usuarios con admin pueden actualizar

        public IActionResult UpdateSlide(int id, [FromBody] Slide slide)
        {
            if (slide == null || id != slide.SlideId)
            {
                return BadRequest("Ingrese los datos correctos"); // Advertencia po si el campo esta vacio o el id no corresponde en la tabla slide
            }

            var existingSlide = _context.Slides.Find(id); // Verifica con la base de datos la existencia de ese ID
            if (existingSlide == null)
            {
                return NotFound("El id no esta registrado");
            }

            // Actualiza los campos del slide existente
            existingSlide.Description = slide.Description;
            existingSlide.URL = slide.URL;
            existingSlide.Status = slide.Status;
            existingSlide.Time = slide.Time;
            existingSlide.ModifiedBy = Environment.MachineName;
            existingSlide.ModifiedDate = DateTime.UtcNow; 

            _context.Slides.Update(existingSlide); // Guarda los datos actualizado
            _context.SaveChanges(); // Guarda

            return Ok(new { message = "Slide actualizado con exito", slide = existingSlide }); // Respuesta 200 la slide se ha actuaizado
        }

        [HttpGet("full-slides")] // Visualizar todas las slides 
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros
        public IActionResult GetSlides()
        {
            var slides = _context.Slides
                .Include(s => s.Board) // Incluye relación con board
                    .ThenInclude(b => b.Category) // Incluye relación categoria
                .Select(s => new
                {
                    SlideId = s.SlideId,
                    Description = s.Description,
                    URL = s.URL,
                    Time = s.Time,
                    Status = s.Status,
                    Board = new
                    {
                        BoardId = s.Board.BoardId,
                        Title = s.Board.Title,
                        Status = s.Board.Status,
                        Category = new
                        {
                            CategoryId = s.Board.Category.CategoryId, 
                            Description = s.Board.Category.Description,
                            CreatedBy = s.Board.Category.CreatedBy,
                            ModifiedBy = s.Board.Category.ModifiedBy, 
                            CreatedDate = s.Board.Category.CreatedDate, 
                            ModifiedDate = s.Board.Category.ModifiedDate, 
                            Status = s.Board.Category.Status
                        }
                    }
                })
                .ToList();

            if (!slides.Any())
            {
                return NotFound("Board NO ecinctrado"); // Si no se encuentra la slide, devuelve error 400
            }

            return Ok(slides); // Devuelve respuesta 200 con el objetoco que se crea para la respuesta
        }

        [HttpGet("list-slides-id")] // Visualiza tableros por id
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros por id
        public IActionResult GetSlideDetails(int id)
        {
            var slide = _context.Slides
                .Include(s => s.Board) // Incluye relación con board
                    .ThenInclude(b => b.Category) // Incluye relación con category
                .Select(s => new
                {
                    SlideId = s.SlideId,
                    Description = s.Description,
                    URL = s.URL,
                    Time = s.Time,
                    Status = s.Status,
                    Board = new
                    {
                        BoardId = s.Board.BoardId,
                        Title = s.Board.Title,
                        Status = s.Status,
                        Category = new
                        {
                            CategoryId = s.Board.Category.CategoryId,
                            Description = s.Board.Category.Description,
                            CreatedBy = s.Board.Category.CreatedBy, 
                            ModifiedBy = s.Board.Category.ModifiedBy, 
                            CreatedDate = s.Board.Category.CreatedDate, 
                            ModifiedDate = s.Board.Category.ModifiedDate,
                            Status = s.Board.Category.Status
                        }
                    }
                })
                .FirstOrDefault(s => s.SlideId == id); // Encargado de mostrar el primer elemento que encuentre respecto al ID

            if (slide == null)
            {
                return NotFound("Ingrese los datos"); // Error 400 por si el id esta en blanco
            }

            return Ok(slide); // Devuelve respuesta 200 con el objetoco que se crea para la respuesta
        }

        [HttpGet("collated-slides")] // Lista de paginas
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

            int totalRecords = await _context.Slides.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

            if (page > totalPages)
            {
                return BadRequest("El número de página excede el total de páginas disponibles."); // Calcula el total de registros y el numero de paginas
            }

            var slides = await _context.Slides // Devuelve segun la pagina soliicitada
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return Ok(new // Respuesta 200 con la info solicitada
            {
                currentPage = page,
                totalPages = totalPages,
                pagesize = pagesize,
                totalRecords = totalRecords,
                data = slides
            });
        }

        [HttpDelete("delete-slide-id")] // Elimina el tablero por ID
        [Authorize(Roles = "Admin")] // Solo el admin tiene el permiso para eliminar 

        public IActionResult DeleteSlide(int id)
        {
            var slide = _context.Slides.Find(id); // Verifica con la conexión si el id exite 
            if (slide == null) 
            {
                return NotFound("Ingrese los datos correspondientes "); // sin el espacio esta vacio responde con error 400
            }

            _context.Slides.Remove(slide); // Elimina
            _context.SaveChanges(); // Guarda
            return Ok(new { message = "Eliminación exitosa " }); // Respuesta de elemininación existosa
        }
    }
}
