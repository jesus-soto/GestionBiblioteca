using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionBiblioteca.Aplicacion.DTO;

namespace GestionBiblioteca.Aplicacion.Interfaces
{
    public interface IServicioLibro
    {
        Task<LibroDTO> CrearLibroAsync(CrearLibroDTO dto);
        Task<ResultadoPaginacionDTO<LibroDTO>> ObtenerLibrosAsync(int pagina, int tamanio, string? titulo, string? nombreAutor);
        Task<LibroDTO?> ObtenerLibroPorIdAsync(Guid id);
        Task<LibroDTO> ActualizarLibroAsync(Guid id, ActualizarLibroDTO DTO);
        Task EliminarLibroAsync(Guid id);
        Task<bool> ValidarIsbnAsync(string isbn);
        Task<IEnumerable<LibroDTO>> CrearLibrosMasivoAsync(Stream archivoStream);
    }
}