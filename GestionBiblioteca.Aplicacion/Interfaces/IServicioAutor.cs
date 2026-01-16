using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBiblioteca.Aplicacion.DTO;

namespace GestionBiblioteca.Aplicacion.Interfaces
{
    public interface IServicioAutor
    {
        Task<AutorDTO> CrearAutorAsync(CrearAutorDTO dto);
        Task<ResultadoPaginacionDTO<AutorDTO>> ObtenerAutoresAsync(int pagina, int tamanio);
        Task<AutorDTO> ActualizarAutorAsync(Guid id, ActualizarAutorDTO dto);
        Task EliminarAutorAsync(Guid id);
    }
}