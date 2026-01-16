using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GestionBiblioteca.Aplicacion.Interfaces;

namespace GestionBiblioteca.Infraestructura.ServiciosExternos
{
    public class ServicioPortadaLibro : IServicioPortadaLibro
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ServicioPortadaLibro(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> ObtenerUrlPortadaAsync(string isbn)
        {
            try
            {
                var cliente = _httpClientFactory.CreateClient();
                var url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
                
                var respuesta = await cliente.GetAsync(url);
                
                if (!respuesta.IsSuccessStatusCode)
                {
                    return string.Empty;
                }

                var contenido = await respuesta.Content.ReadAsStringAsync();
                
                if (string.IsNullOrWhiteSpace(contenido) || contenido == "{}")
                {
                    return string.Empty;
                }

                using var documento = JsonDocument.Parse(contenido);
                var root = documento.RootElement;
                
                var clave = $"ISBN:{isbn}";
                if (root.TryGetProperty(clave, out var libroElement))
                {
                    if (libroElement.TryGetProperty("cover", out var coverElement))
                    {
                        if (coverElement.TryGetProperty("large", out var largeElement))
                        {
                            return largeElement.GetString() ?? string.Empty;
                        }
                        if (coverElement.TryGetProperty("medium", out var mediumElement))
                        {
                            return mediumElement.GetString() ?? string.Empty;
                        }
                        if (coverElement.TryGetProperty("small", out var smallElement))
                        {
                            return smallElement.GetString() ?? string.Empty;
                        }
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener portada para ISBN {isbn}: {ex.Message}");
                return string.Empty;
            }
        }
    }
}