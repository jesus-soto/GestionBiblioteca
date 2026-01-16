using Xunit;
using FluentAssertions;
using GestionBiblioteca.Aplicacion.Servicios;

namespace GestionBiblioteca.Pruebas.Servicios
{
    public class ServicioNormalizacionTests
    {
        private readonly ServicioNormalizacion _servicio;

        public ServicioNormalizacionTests()
        {
            _servicio = new ServicioNormalizacion();
        }

        [Fact]
        public void NormalizarTexto_DebeConvertirAMayusculas()
        {
            // Arrange
            var texto = "TextO a Normalizar";

            // Act
            var resultado = _servicio.NormalizarTexto(texto);

            // Assert
            resultado.Should().Be("TEXTO A NORMALIZAR");
        }

        [Fact]
        public void NormalizarTexto_DebeEliminarNumeros()
        {
            // Arrange
            var texto = "Libro 123 del año 2024";

            // Act
            var resultado = _servicio.NormalizarTexto(texto);

            // Assert
            resultado.Should().Be("LIBRO DEL ANO");
        }

        [Fact]
        public void NormalizarTexto_DebeEliminarTildes()
        {
            // Arrange
            var texto = "Programación Básica";

            // Act
            var resultado = _servicio.NormalizarTexto(texto);

            // Assert
            resultado.Should().Be("PROGRAMACION BASICA");
        }

        [Fact]
        public void NormalizarTexto_DebeEliminarEnie()
        {
            // Arrange
            var texto = "Español y niño";

            // Act
            var resultado = _servicio.NormalizarTexto(texto);

            // Assert
            resultado.Should().Be("ESPANOL Y NINO");
        }

        [Fact]
        public void NormalizarTexto_DebeReemplazarEspaciosMultiples()
        {
            // Arrange
            var texto = "texto   con    espacios";

            // Act
            var resultado = _servicio.NormalizarTexto(texto);

            // Assert
            resultado.Should().Be("TEXTO CON ESPACIOS");
        }
    
    }
}