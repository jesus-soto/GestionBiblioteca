using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;
using GestionBiblioteca.Dominio.Entidades;
using GestionBiblioteca.Dominio.Interfaces;

namespace GestionBiblioteca.Aplicacion.Servicios
{
    public class ServicioLibro : IServicioLibro
    {
        private readonly IRepositorioLibro _repositorioLibro;
        private readonly IRepositorioAutor _repositorioAutor;
        private readonly IServicioNormalizacion _servicioNormalizacion;
        private readonly IServicioValidacionIsbn _servicioValidacionIsbn;
        private readonly IServicioPortadaLibro _servicioPortadaLibro;

        public ServicioLibro(
            IRepositorioLibro repositorioLibro,
            IRepositorioAutor repositorioAutor,
            IServicioNormalizacion servicioNormalizacion,
            IServicioValidacionIsbn servicioValidacionIsbn,
            IServicioPortadaLibro servicioPortadaLibro)
        {
            _repositorioLibro = repositorioLibro;
            _repositorioAutor = repositorioAutor;
            _servicioNormalizacion = servicioNormalizacion;
            _servicioValidacionIsbn = servicioValidacionIsbn;
            _servicioPortadaLibro = servicioPortadaLibro;
        }

        public async Task<LibroDTO> CrearLibroAsync(CrearLibroDTO dto)
        {
            // Validar ISBN con servicio SOAP
            var isbnValido = await _servicioValidacionIsbn.ValidarIsbnAsync(dto.Isbn);
            if (!isbnValido)
            {
                throw new InvalidOperationException($"El ISBN {dto.Isbn} no es válido");
            }

            // Verificar que el autor existe
            var autor = await _repositorioAutor.ObtenerPorIdAsync(dto.AuthorId);
            if (autor == null)
            {
                throw new InvalidOperationException($"El autor con ID {dto.AuthorId} no existe");
            }

            // Obtener URL de portada
            var urlPortada = await _servicioPortadaLibro.ObtenerUrlPortadaAsync(dto.Isbn);

            // Normalizar título
            var TitleNormalizado = _servicioNormalizacion.NormalizarTexto(dto.Title);

            var libro = new Book
            {
                Id = Guid.NewGuid(),
                Isbn = dto.Isbn,
                Title = TitleNormalizado,
                CoverUrl = urlPortada,
                PublicationYear = dto.PublicationYear,
                AuthorId = dto.AuthorId
            };

            var libroCreado = await _repositorioLibro.AgregarAsync(libro);

            return MapearALibroDTO(libroCreado);
        }

        public async Task<ResultadoPaginacionDTO<LibroDTO>> ObtenerLibrosAsync(int pagina, int tamanio, string? Title, string? nombreAutor)
        {
            var libros = await _repositorioLibro.ObtenerTodosAsync(pagina, tamanio, Title, nombreAutor);
            var total = await _repositorioLibro.ContarAsync(Title, nombreAutor);

            return new ResultadoPaginacionDTO<LibroDTO>
            {
                Datos = libros.Select(MapearALibroDTO),
                PaginaActual = pagina,
                TamanioPagina = tamanio,
                TotalElementos = total,
                TotalPaginas = (int)Math.Ceiling(total / (double)tamanio)
            };
        }

        public async Task<LibroDTO?> ObtenerLibroPorIdAsync(Guid id)
        {
            var libro = await _repositorioLibro.ObtenerPorIdAsync(id);
            return libro != null ? MapearALibroDTO(libro) : null;
        }

        public async Task<LibroDTO> ActualizarLibroAsync(Guid id, ActualizarLibroDTO dto)
        {
            var libro = await _repositorioLibro.ObtenerPorIdAsync(id);
            if (libro == null)
            {
                throw new InvalidOperationException($"El libro con ID {id} no existe");
            }

            if (!string.IsNullOrEmpty(dto.Title))
            {
                libro.Title = _servicioNormalizacion.NormalizarTexto(dto.Title);
            }

            if (dto.PublicationYear.HasValue)
            {
                libro.PublicationYear = dto.PublicationYear.Value;
            }

            if (dto.AuthorId.HasValue)
            {
                var autor = await _repositorioAutor.ObtenerPorIdAsync(dto.AuthorId.Value);
                if (autor == null)
                {
                    throw new InvalidOperationException($"El autor con ID {dto.AuthorId.Value} no existe");
                }
                libro.AuthorId = dto.AuthorId.Value;
            }

            await _repositorioLibro.ActualizarAsync(libro);

            return MapearALibroDTO(libro);
        }

        public async Task EliminarLibroAsync(Guid id)
        {
            var libro = await _repositorioLibro.ObtenerPorIdAsync(id);
            if (libro == null)
            {
                throw new InvalidOperationException($"El libro con ID {id} no existe");
            }

            await _repositorioLibro.EliminarAsync(id);
        }

        public async Task<bool> ValidarIsbnAsync(string isbn)
        {
            return await _servicioValidacionIsbn.ValidarIsbnAsync(isbn);
        }

        public async Task<IEnumerable<LibroDTO>> CrearLibrosMasivoAsync(Stream archivoStream)
        {
            var librosCreados = new List<LibroDTO>();
            
            using var reader = new StreamReader(archivoStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null
            });

            var registros = csv.GetRecords<LibroCsvDto>().ToList();

            foreach (var registro in registros)
            {
                try
                {
                    // Buscar o crear autor
                    var nombreAutorNormalizado = _servicioNormalizacion.NormalizarTexto(registro.Name);
                    var autor = await _repositorioAutor.ObtenerPorNombreAsync(nombreAutorNormalizado);
                    
                    if (autor == null)
                    {
                        autor = await _repositorioAutor.AgregarAsync(new Author
                        {
                            Id = Guid.NewGuid(),
                            Name = nombreAutorNormalizado
                        });
                    }

                    // Crear libro
                    var LibroDTO = new CrearLibroDTO
                    {
                        Isbn = registro.Isbn,
                        Title = registro.Title,
                        PublicationYear = registro.PublicationYear,
                        AuthorId = autor.Id
                    };

                    var libroCreado = await CrearLibroAsync(LibroDTO);
                    librosCreados.Add(libroCreado);
                }
                catch (Exception ex)
                {
                    // Log error pero continuar con siguiente registro
                    Console.WriteLine($"Error al procesar libro ISBN {registro.Isbn}: {ex.Message}");
                }
            }

            return librosCreados;
        }

        private LibroDTO MapearALibroDTO(Book libro)
        {
            return new LibroDTO
            {
                Id = libro.Id,
                Isbn = libro.Isbn,
                Title = libro.Title,
                CoverUrl = libro.CoverUrl,
                PublicationYear = libro.PublicationYear,
                AuthorId = libro.AuthorId,
                AuthorName = libro.Author?.Name
            };
        }

        private class LibroCsvDto
        {
            public string Isbn { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public int PublicationYear { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}