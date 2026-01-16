using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Dominio.Entidades;

namespace GestionBiblioteca.Dominio.Interfaces
{
    public interface IRepositorioAutor
    {
        //Task<IEnumerable<Author>> ObtenerTodosAsync();
        Task<IEnumerable<Author>> ObtenerTodosAsync(int pagina, int tamanio);
        Task<Author?> ObtenerPorNombreAsync(string nombre);
        Task<Author?> ObtenerPorIdAsync(Guid id);
        Task<Author> AgregarAsync(Author autor);
        Task ActualizarAsync(Author autor);
        Task EliminarAsync(Guid id);
        Task<int> ContarAsync();
    
    }
}