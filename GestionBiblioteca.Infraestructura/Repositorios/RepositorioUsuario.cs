using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionBiblioteca.Dominio.Entidades;
using GestionBiblioteca.Dominio.Interfaces;
using GestionBiblioteca.Infraestructura.Persistencia;

namespace GestionBiblioteca.Infraestructura.Repositorios
{
    public class RepositorioUsuario: IRepositorioUsuario
    {
        private readonly BibliotecaDbContext _context;

        public RepositorioUsuario(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
        }
    }
}