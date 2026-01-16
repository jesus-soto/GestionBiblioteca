using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;
using GestionBiblioteca.Aplicacion.Servicios;
using GestionBiblioteca.Dominio.Entidades;
using GestionBiblioteca.Dominio.Interfaces;

namespace GestionBiblioteca.Pruebas.Servicios
{
    public class ServicioAutorTests
    {
        private readonly Mock<IRepositorioAutor> _repositorioMock;
        private readonly Mock<IServicioNormalizacion> _normalizacionMock;
        private readonly ServicioAutor _servicio;

        public ServicioAutorTests()
        {
            _repositorioMock = new Mock<IRepositorioAutor>();
            _normalizacionMock = new Mock<IServicioNormalizacion>();
            _servicio = new ServicioAutor(_repositorioMock.Object, _normalizacionMock.Object);
        }

        [Fact]
        public async Task CrearAutorAsync_DebeNormalizarNombre()
        {
            // Arrange
            var dto = new CrearAutorDTO { Name = "Gabriel García Márquez" };
            var nombreNormalizado = "GABRIEL GARCIA MARQUEZ";

            _normalizacionMock
                .Setup(x => x.NormalizarTexto(dto.Name))
                .Returns(nombreNormalizado);

            _repositorioMock
                .Setup(x => x.ObtenerPorNombreAsync(nombreNormalizado))
                .ReturnsAsync((Author?)null);

            _repositorioMock
                .Setup(x => x.AgregarAsync(It.IsAny<Author>()))
                .ReturnsAsync((Author a) => a);

            // Act
            var resultado = await _servicio.CrearAutorAsync(dto);

            // Assert
            resultado.Name.Should().Be(nombreNormalizado);
            _normalizacionMock.Verify(x => x.NormalizarTexto(dto.Name), Times.Once);
        }

        [Fact]
        public async Task CrearAutorAsync_DebeLanzarExcepcion_CuandoAutorYaExiste()
        {
            // Arrange
            var dto = new CrearAutorDTO { Name = "Gabriel García Márquez" };
            var nombreNormalizado = "GABRIEL GARCIA MARQUEZ";

            _normalizacionMock
                .Setup(x => x.NormalizarTexto(dto.Name))
                .Returns(nombreNormalizado);

            _repositorioMock
                .Setup(x => x.ObtenerPorNombreAsync(nombreNormalizado))
                .ReturnsAsync(new Author { Id = Guid.NewGuid(), Name = nombreNormalizado });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _servicio.CrearAutorAsync(dto));
        }
    
    }
}