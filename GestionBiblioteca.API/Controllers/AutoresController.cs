using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;

namespace GestionBiblioteca.API.Controllers
{
    [Authorize]
    [Route("api/authors")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly IServicioAutor _servicioAutor;

        public AutoresController(IServicioAutor servicioAutor)
        {
            _servicioAutor = servicioAutor;
        }

        /// <summary>
        /// Obtener todos los autores con paginación
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultadoPaginacionDTO<AutorDTO>>> ObtenerAutores(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanio = 10)
        {
            var resultado = await _servicioAutor.ObtenerAutoresAsync(pagina, tamanio);
            return Ok(resultado);
        }

        /// <summary>
        /// Crear un nuevo autor
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AutorDTO>> CrearAutor([FromBody] CrearAutorDTO dto)
        {
            try
            {
                var autor = await _servicioAutor.CrearAutorAsync(dto);
                return CreatedAtAction(nameof(ObtenerAutores), new { id = autor.Id }, autor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar parcialmente un autor
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<AutorDTO>> ActualizarAutor(Guid id, [FromBody] ActualizarAutorDTO dto)
        {
            try
            {
                var autor = await _servicioAutor.ActualizarAutorAsync(id, dto);
                return Ok(autor);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar un autor por ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarAutor(Guid id)
        {
            try
            {
                await _servicioAutor.EliminarAutorAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}