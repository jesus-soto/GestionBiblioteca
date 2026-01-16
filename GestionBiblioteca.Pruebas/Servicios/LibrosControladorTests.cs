using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using GestionBiblioteca.API.Controllers;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;

namespace GestionBiblioteca.Pruebas.Servicios
{
    public class LibrosControladorTests
    {
        private readonly Mock<IServicioLibro> _servicioMock;
        private readonly LibrosController _controlador;

        public LibrosControladorTests()
        {
            _servicioMock = new Mock<IServicioLibro>();
            _controlador = new LibrosController(_servicioMock.Object);
        }

        [Fact]
        public async Task ObtenerLibroPorId_DebeRetornarOk_CuandoLibroExiste()
        {
            // Arrange
            var libroId = Guid.NewGuid();
            var libroDto = new LibroDTO
            {
                Id = libroId,
                Isbn = "9780123456789",
                Title = "EL QUIJOTE",
                CoverUrl = "http://ejemplo.com/portada.jpg",
                PublicationYear = 1605,
                AuthorId = Guid.NewGuid(),
                AuthorName = "MIGUEL DE CERVANTES"
            };

            _servicioMock
                .Setup(x => x.ObtenerLibroPorIdAsync(libroId))
                .ReturnsAsync(libroDto);

            // Act
            var resultado = await _controlador.ObtenerLibroPorId(libroId);

            // Assert
            resultado.Result.Should().BeOfType<OkObjectResult>();
            var okResult = resultado.Result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(libroDto);
        }

        [Fact]
        public async Task ObtenerLibroPorId_DebeRetornarNotFound_CuandoLibroNoExiste()
        {
            // Arrange
            var libroId = Guid.NewGuid();

            _servicioMock
                .Setup(x => x.ObtenerLibroPorIdAsync(libroId))
                .ReturnsAsync((LibroDTO?)null);

            // Act
            var resultado = await _controlador.ObtenerLibroPorId(libroId);

            // Assert
            resultado.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CrearLibro_DebeRetornarCreated_CuandoEsExitoso()
        {
            // Arrange
            var crearDto = new CrearLibroDTO
            {
                Isbn = "9780123456789",
                Title = "El Quijote",
                PublicationYear = 1605,
                AuthorId = Guid.NewGuid()
            };

            var libroCreado = new LibroDTO
            {
                Id = Guid.NewGuid(),
                Isbn = crearDto.Isbn,
                Title = "EL QUIJOTE",
                CoverUrl = "http://ejemplo.com/portada.jpg",
                PublicationYear = crearDto.PublicationYear,
                AuthorId = crearDto.AuthorId
            };

            _servicioMock
                .Setup(x => x.CrearLibroAsync(crearDto))
                .ReturnsAsync(libroCreado);

            // Act
            var resultado = await _controlador.CrearLibro(crearDto);

            // Assert
            resultado.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = resultado.Result as CreatedAtActionResult;
            createdResult?.Value.Should().BeEquivalentTo(libroCreado);
        }
    
    }
}