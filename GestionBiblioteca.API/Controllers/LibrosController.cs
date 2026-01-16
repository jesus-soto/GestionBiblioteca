using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;

namespace GestionBiblioteca.API.Controllers
{
    [Authorize]
    [Route("api/books")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly IServicioLibro _servicioLibro;

        public LibrosController(IServicioLibro servicioLibro)
        {
            _servicioLibro = servicioLibro;
        }

        /// <summary>
        /// Obtener todos los libros con paginación y filtros opcionales
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultadoPaginacionDTO<LibroDTO>>> ObtenerLibros(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanio = 10,
            [FromQuery] string? titulo = null,
            [FromQuery] string? nombreAutor = null)
        {
            var resultado = await _servicioLibro.ObtenerLibrosAsync(pagina, tamanio, titulo, nombreAutor);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtener un libro por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LibroDTO>> ObtenerLibroPorId(Guid id)
        {
            var libro = await _servicioLibro.ObtenerLibroPorIdAsync(id);

            if (libro == null)
            {
                return NotFound(new { mensaje = $"Libro con ID {id} no encontrado" });
            }

            return Ok(libro);
        }

        /// <summary>
        /// Crear un nuevo libro
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LibroDTO>> CrearLibro([FromBody] CrearLibroDTO dto)
        {
            try
            {
                var libro = await _servicioLibro.CrearLibroAsync(dto);
                return CreatedAtAction(nameof(ObtenerLibroPorId), new { id = libro.Id }, libro);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar parcialmente un libro
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<LibroDTO>> ActualizarLibro(Guid id, [FromBody] ActualizarLibroDTO dto)
        {
            try
            {
                var libro = await _servicioLibro.ActualizarLibroAsync(id, dto);
                return Ok(libro);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar un libro por ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarLibro(Guid id)
        {
            try
            {
                await _servicioLibro.EliminarLibroAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Validar un ISBN
        /// </summary>
        [HttpGet("validation/{isbn}")]
        public async Task<ActionResult<object>> ValidarIsbn(string isbn)
        {
            var esValido = await _servicioLibro.ValidarIsbnAsync(isbn);
            return Ok(new { isbn, esValido });
        }

        /// <summary>
        /// Crear libros de forma masiva desde archivo CSV
        /// </summary>
        [HttpPost("masive")]
        public async Task<ActionResult<IEnumerable<LibroDTO>>> CrearLibrosMasivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest(new { mensaje = "Debe proporcionar un archivo CSV válido" });
            }

            if (!archivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { mensaje = "El archivo debe tener extensión .csv" });
            }

            try
            {
                using var stream = archivo.OpenReadStream();
                var libros = await _servicioLibro.CrearLibrosMasivoAsync(stream);
                return Ok(new { mensaje = "Libros creados exitosamente", libros });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = $"Error al procesar archivo: {ex.Message}" });
            }
        }
    }
}