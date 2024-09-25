using BoardContr0l.Data;
using BoardContr0l.Dto;
using BoardContr0l.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace BoardContr0l.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Restringe el acceso a usuarios no autenticados
    public class BoardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BoardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-board")] // Crea un tablero 
        [Authorize(Roles = "Admin")] // Solo los usuarios con admin pueden crear


        public IActionResult CreateBoard([FromBody] Board board) // Relación a tabla de Board y conexión de sus propiedades
        {
            if (board == null)
            {
                return BadRequest(); // Si el board es nulo, devulve un error 400
            }

            // Valores que se ingresaran automatizados 
            board.CreatedBy = Environment.MachineName;
            board.CreatedDate = DateTime.UtcNow;
            board.ModifiedBy = null;  
            board.ModifiedDate = null;

            _context.Boards.Add(board); // Agrega os datos ingresados a ala base de datos
            _context.SaveChanges(); // Guarda cambios

            return CreatedAtAction(nameof(GetBoardDetails), new { id = board.BoardId }, board); // Retorna respuesta 201 con detalles del tablero que se creo
        }

        [HttpPut("Update-board")] // Actualiza tablero 
        [Authorize(Roles = "Admin")] // Solo los usuarios con admin pueden actualizar

        public IActionResult UpdateBoard(int id, [FromBody] Board board)
        {
            if (board == null || id != board.BoardId)
            {
                return BadRequest(); // Si el board o el id no coinciden devuelve error  400
            }

            var existingBoard = _context.Boards.Find(id);  // Verifica la existencia del board en la base de datoos
            if (existingBoard == null)
            {
                return NotFound($"El Board con ID {board.BoardId} no existe."); // Si el id no existe, devuelve error 400

            }

            // Valores que se ingresaran automatizados 
            existingBoard.Status = board.Status;
            existingBoard.Title = board.Title;
            existingBoard.ModifiedBy = Environment.MachineName; 
            existingBoard.ModifiedDate = DateTime.UtcNow;

            _context.Boards.Update(existingBoard); // Actualiza el tablero con los datos que se ingresaron
            _context.SaveChanges(); // Guarda cambios

            return Ok(new { message = "El tablero se ha actualizado con exito", board = existingBoard }); // Respuesta 200 el tablero se ha actuaizado

        }

        [HttpGet("full-boards")] // Visualiza todos los boards ingresados
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros
        public IActionResult GetBoards()
        {
            var boards = _context.Boards
                .Include(b => b.Category) // Incluye la categoría relacionada al board
                .ToList();

            // Objeto que devolvera y relación a cateogria 
            var boardDtos = boards.Select(b => new
            {
                BoardId = b.BoardId,
                Title = b.Title,
                Status = b.Status, 
                CreatedBy = b.CreatedBy, 
                ModifiedBy = b.ModifiedBy,
                CreatedDate = b.CreatedDate, 
                ModifiedDate = b.ModifiedDate,

                Category = new
                {
                    CategoryId = b.Category.CategoryId,
                    Description = b.Category.Description,
                    CreatedBy = b.Category.CreatedBy, 
                    ModifiedBy = b.Category.ModifiedBy, 
                    CreatedDate = b.Category.CreatedDate, 
                    ModifiedDate = b.Category.ModifiedDate, 
                    Status = b.Category.Status 
                }
            }).ToList();

            if (!boardDtos.Any())
            {
                return NotFound("Board NO econctrado"); // Si no se encuentran tableros, devuelve error 400
            }
            return Ok(boardDtos); // Devuelve respuesta 200 con el objetoco que se crea para la respuesta
        }


        [HttpGet("list-board-id")] // Visualiza tableros por id 
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros por id

        public IActionResult GetBoardDetails(int id)
        {
            var board = _context.Boards
                .Include(b => b.Category) // Incluye la categoría relacionada al board
                .FirstOrDefault(b => b.BoardId == id); 

            if (board == null) // Verifica la existencia del id del board
            {
                return NotFound($"El Board con ID {board.BoardId} no existe."); // Si el id no existe, devuelve error 400
            }

            // Objeto que devolvera y relación a cateogria 
            var boardDto = new
            {
                BoardId = board.BoardId,
                Title = board.Title,
                Status = board.Status, 
                Category = new
                {
                    CategoryId = board.Category.CategoryId,
                    Description = board.Category.Description,
                    CreatedBy = board.Category.CreatedBy, 
                    ModifiedBy = board.Category.ModifiedBy,
                    CreatedDate = board.Category.CreatedDate,
                    ModifiedDate = board.Category.ModifiedDate,
                    Status = board.Category.Status
                }
            };

            return Ok(boardDto); // Devuelve respuesta 200 con el objetoco que se crea para la respuesta
        }

        [HttpGet("list-boards-by-category")] // Visualiza todos los tableros asociados a una categoría
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar tableros por categoría
        public IActionResult GetBoardsByCategory(int categoryId)
        {
            var boards = _context.Boards
                .Include(b => b.Category) // Incluye la categoría relacionada al board
                .Where(b => b.Category.CategoryId == categoryId) // Filtra los boards por el categoryId
                .ToList();

            if (!boards.Any()) // Si no hay tableros respecto a la categoria
            {
                return NotFound($"No se encontraron tableros para la categoría con ID {categoryId}.");
            }

            // Objeto que devolverá con relación a la categoría
            var boardDtos = boards.Select(b => new
            {
                BoardId = b.BoardId,
                Title = b.Title,
                Status = b.Status,
                CreatedBy = b.CreatedBy,
                ModifiedBy = b.ModifiedBy,
                CreatedDate = b.CreatedDate,
                ModifiedDate = b.ModifiedDate,
                Category = new
                {
                    CategoryId = b.Category.CategoryId,
                    Description = b.Category.Description,
                    CreatedBy = b.Category.CreatedBy,
                    ModifiedBy = b.Category.ModifiedBy,
                    CreatedDate = b.Category.CreatedDate,
                    ModifiedDate = b.Category.ModifiedDate,
                    Status = b.Category.Status
                }
            }).ToList();

            return Ok(boardDtos); // Respuesta 200 con los tableros de la categoría
        }



        [HttpGet("collated-boards")] // Lista de paginas
        [Authorize(Roles = "Admin, User")] // Tanto usuarios como administrador pueden visualizar a gusto 
        public async Task<IActionResult> GetPaginatedBoards([FromQuery] int page = 1, [FromQuery] int pagesize= 8)
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
                pagesize= pagesize,
                totalRecords = totalRecords,
                data = boards
            });
        }

        [HttpDelete("delete-board-id")] // Elimina tablero  por id 
        [Authorize(Roles = "Admin")] // Solo el admin tiene el permiso para eliminar 

        public IActionResult DeleteBoard(int id)
        {
            var board = _context.Boards.Include(b => b.Slides).FirstOrDefault(b => b.BoardId == id); // Elimina por id del tablero

            if (board == null) // Si no encuentra el boar por el id, devuelve el error 
            {
                return NotFound("El board no ha sido encontrado");
            }

            if (board.Slides.Any()) // Elimina todos los slides asociados a ese id del board
            {
                _context.Slides.RemoveRange(board.Slides);
            }

            _context.Boards.Remove(board); // Elimina
            _context.SaveChanges(); // guarda

            return Ok(new { message = "Eliminación exitosa " }); // Respuesta de elemininación existosa
        }

    }
}
