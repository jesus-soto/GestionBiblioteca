using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Aplicacion.Interfaces
{
    public interface IServicioPortadaLibro
    {
        Task<string> ObtenerUrlPortadaAsync(string isbn);

    }
}