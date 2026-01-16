using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;
using GestionBiblioteca.Dominio.Entidades;
using GestionBiblioteca.Dominio.Interfaces;

namespace GestionBiblioteca.Aplicacion.Servicios
{
    public class ServicioAutor : IServicioAutor
    {
        private readonly IRepositorioAutor _repositorioAutor;
        private readonly IServicioNormalizacion _servicioNormalizacion;

        public ServicioAutor(
            IRepositorioAutor repositorioAutor,
            IServicioNormalizacion servicioNormalizacion)
        {
            _repositorioAutor = repositorioAutor;
            _servicioNormalizacion = servicioNormalizacion;
        }

        public async Task<AutorDTO> CrearAutorAsync(CrearAutorDTO dto)
        {
            var nombreNormalizado = _servicioNormalizacion.NormalizarTexto(dto.Name);

            var autorExistente = await _repositorioAutor.ObtenerPorNombreAsync(nombreNormalizado);
            if (autorExistente != null)
            {
                throw new InvalidOperationException($"Ya existe un autor con el nombre '{nombreNormalizado}'");
            }

            var autor = new Author
            {
                Id = Guid.NewGuid(),
                Name = nombreNormalizado
            };

            var autorCreado = await _repositorioAutor.AgregarAsync(autor);

            return MapearAAutorDTO(autorCreado);
        }

        public async Task<ResultadoPaginacionDTO<AutorDTO>> ObtenerAutoresAsync(int pagina, int tamanio)
        {
            var autores = await _repositorioAutor.ObtenerTodosAsync(pagina, tamanio);
            var total = await _repositorioAutor.ContarAsync();

            return new ResultadoPaginacionDTO<AutorDTO>
            {
                Datos = autores.Select(MapearAAutorDTO),
                PaginaActual = pagina,
                TamanioPagina = tamanio,
                TotalElementos = total,
                TotalPaginas = (int)Math.Ceiling(total / (double)tamanio)
            };
        }

        public async Task<AutorDTO> ActualizarAutorAsync(Guid id, ActualizarAutorDTO dto)
        {
            var autor = await _repositorioAutor.ObtenerPorIdAsync(id);
            if (autor == null)
            {
                throw new InvalidOperationException($"El autor con ID {id} no existe");
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                var nombreNormalizado = _servicioNormalizacion.NormalizarTexto(dto.Name);

                var autorExistente = await _repositorioAutor.ObtenerPorNombreAsync(nombreNormalizado);
                if (autorExistente != null && autorExistente.Id != id)
                {
                    throw new InvalidOperationException($"Ya existe otro autor con el nombre '{nombreNormalizado}'");
                }

                autor.Name = nombreNormalizado;
            }

            await _repositorioAutor.ActualizarAsync(autor);

            return MapearAAutorDTO(autor);
        }

        public async Task EliminarAutorAsync(Guid id)
        {
            var autor = await _repositorioAutor.ObtenerPorIdAsync(id);
            if (autor == null)
            {
                throw new InvalidOperationException($"El autor con ID {id} no existe");
            }

            await _repositorioAutor.EliminarAsync(id);
        }

        private AutorDTO MapearAAutorDTO(Author autor)
        {
            return new AutorDTO
            {
                Id = autor.Id,
                Name = autor.Name
            };
        }
    }
}