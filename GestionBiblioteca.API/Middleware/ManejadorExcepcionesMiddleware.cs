using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GestionBiblioteca.API.Middleware
{
    public class ManejadorExcepcionesMiddleware
    {
       private readonly RequestDelegate _next;
        private readonly ILogger<ManejadorExcepcionesMiddleware> _logger;

        public ManejadorExcepcionesMiddleware(
            RequestDelegate next,
            ILogger<ManejadorExcepcionesMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió una excepción no manejada");
                await ManejadorExcepcionAsync(context, ex);
            }
        }

        private static async Task ManejadorExcepcionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            
            var statusCode = exception switch
            {
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var problemDetails = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = ObtenerTituloError(statusCode),
                status = statusCode,
                detail = exception.Message,
                instance = context.Request.Path.ToString(),
                timestamp = DateTime.UtcNow
            };

            var opciones = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(problemDetails, opciones);
            await context.Response.WriteAsync(json);
        }

        private static string ObtenerTituloError(int statusCode)
        {
            return statusCode switch
            {
                400 => "Solicitud Incorrecta",
                401 => "No Autorizado",
                404 => "No Encontrado",
                500 => "Error Interno del Servidor",
                _ => "Error"
            };
        }
    }
}