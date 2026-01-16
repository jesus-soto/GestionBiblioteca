using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Dominio.Entidades;

namespace GestionBiblioteca.Dominio.Interfaces
{
    public interface IRepositorioLibro
    {
        //Task<IEnumerable<Book>> ObtenerTodosAsync();
        Task<Book?> ObtenerPorIdAsync(Guid id);
        Task<IEnumerable<Book>> ObtenerTodosAsync(int pagina, int tamanio, string? titulo, string? nombreAutor);
        Task<Book> AgregarAsync(Book libro);
        Task ActualizarAsync(Book libro);
        Task EliminarAsync(Guid id);    
        //Task<int> ContarLibrosPorAutorAsync(Guid authorId);
        Task<int> ContarAsync(string? titulo, string? nombreAutor);

    }
}