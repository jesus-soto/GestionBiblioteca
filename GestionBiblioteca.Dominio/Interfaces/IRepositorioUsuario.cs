using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBiblioteca.Dominio.Entidades;

namespace GestionBiblioteca.Dominio.Interfaces
{
    public interface IRepositorioUsuario
    {
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
    }
}