using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Aplicacion.DTO
{
    public class ResultadoPaginacionDTO<T>
    {
        public IEnumerable<T> Datos { get; set; } = new List<T>();
        public int PaginaActual { get; set; }
        public int TamanioPagina { get; set; }
        public int TotalElementos { get; set; }
        public int TotalPaginas { get; set; }
    }
}