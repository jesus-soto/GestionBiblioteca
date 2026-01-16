using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionBiblioteca.Dominio.Entidades;
using GestionBiblioteca.Dominio.Interfaces;
using GestionBiblioteca.Infraestructura.Persistencia;

namespace GestionBiblioteca.Infraestructura.Repositorios
{
    public class RepositorioAutor: IRepositorioAutor
    {
        private readonly BibliotecaDbContext _context;

        public RepositorioAutor(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Author?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Author?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.Name == nombre);
        }

        public async Task<IEnumerable<Author>> ObtenerTodosAsync(int pagina, int tamanio)
        {
            return await _context.Authors
                .OrderBy(a => a.Name)
                .Skip((pagina - 1) * tamanio)
                .Take(tamanio)
                .ToListAsync();
        }

        public async Task<Author> AgregarAsync(Author autor)
        {
            _context.Authors.Add(autor);
            await _context.SaveChangesAsync();
            return autor;
        }

        public async Task ActualizarAsync(Author autor)
        {
            _context.Authors.Update(autor);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var autor = await _context.Authors.FindAsync(id);
            if (autor != null)
            {
                _context.Authors.Remove(autor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ContarAsync()
        {
            return await _context.Authors.CountAsync();
        }
    }
}