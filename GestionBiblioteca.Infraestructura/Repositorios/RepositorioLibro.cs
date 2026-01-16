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
    public class RepositorioLibro : IRepositorioLibro
    {
        private readonly BibliotecaDbContext _context;

        public RepositorioLibro(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Books
                .Include(l => l.Author)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Book>> ObtenerTodosAsync(int pagina, int tamanio, string? titulo, string? nombreAutor)
        {
            var query = _context.Books.Include(l => l.Author).AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(l => l.Title.Contains(titulo));
            }

            if (!string.IsNullOrEmpty(nombreAutor))
            {
                query = query.Where(l => l.Author != null && l.Author.Name.Contains(nombreAutor));
            }

            return await query
                .OrderBy(l => l.Title)
                .Skip((pagina - 1) * tamanio)
                .Take(tamanio)
                .ToListAsync();
        }

        public async Task<Book> AgregarAsync(Book libro)
        {
            _context.Books.Add(libro);
            await _context.SaveChangesAsync();
            return libro;
        }

        public async Task ActualizarAsync(Book libro)
        {
            _context.Books.Update(libro);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var libro = await _context.Books.FindAsync(id);
            if (libro != null)
            {
                _context.Books.Remove(libro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ContarAsync(string? titulo, string? nombreAutor)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(l => l.Title.Contains(titulo));
            }

            if (!string.IsNullOrEmpty(nombreAutor))
            {
                query = query.Where(l => l.Author != null && l.Author.Name.Contains(nombreAutor));
            }

            return await query.CountAsync();
        }
    }
    
}