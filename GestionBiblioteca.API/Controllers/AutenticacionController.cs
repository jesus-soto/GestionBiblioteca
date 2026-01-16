using Microsoft.AspNetCore.Mvc;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;

namespace GestionBiblioteca.API.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly IServicioAutenticacion _servicioAutenticacion;

        public AutenticacionController(IServicioAutenticacion servicioAutenticacion)
        {
            _servicioAutenticacion = servicioAutenticacion;
        }

        /// <summary>
        /// Generar un token JWT para autenticación
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<TokenDTO>> Login([FromQuery] string nombreUsuario, [FromQuery] string clave)
        {
            var loginDto = new LoginDTO
            {
                NombreUsuario = nombreUsuario,
                Clave = clave
            };

            var token = await _servicioAutenticacion.AutenticarAsync(loginDto);

            if (token == null)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas" });
            }

            return Ok(token);
        }
    }
}