using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using GestionBiblioteca.Aplicacion.DTO;
using GestionBiblioteca.Aplicacion.Interfaces;
using GestionBiblioteca.Dominio.Interfaces;

namespace GestionBiblioteca.Infraestructura.Autenticacion
{
    public class ServicioAutenticacion : IServicioAutenticacion
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IConfiguration _configuration;

        public ServicioAutenticacion(
            IRepositorioUsuario repositorioUsuario,
            IConfiguration configuration)
        {
            _repositorioUsuario = repositorioUsuario;
            _configuration = configuration;
        }

        public async Task<TokenDTO?> AutenticarAsync(LoginDTO loginDto)
        {
            var usuario = await _repositorioUsuario.ObtenerPorNombreUsuarioAsync(loginDto.NombreUsuario);
            
            if (usuario == null)
            {
                return null;
            }

            var contraseniaHash = HashPassword(loginDto.Clave);
            
            if (usuario.ClaveHash != contraseniaHash)
            {
                return null;
            }

            // Generar JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var claveSecreta = _configuration["Jwt:Key"] ?? "ClaveSecretaSuperSeguraParaJWT2024!@#$%^&*()";
            var key = Encoding.ASCII.GetBytes(claveSecreta);
            
            var expiracion = DateTime.UtcNow.AddHours(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario)
                }),
                Expires = expiracion,
                Issuer = _configuration["Jwt:Issuer"] ?? "GestionBiblioteca",
                Audience = _configuration["Jwt:Audience"] ?? "GestionBibliotecaAPI",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new TokenDTO
            {
                Token = tokenString,
                Expiracion = expiracion
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}