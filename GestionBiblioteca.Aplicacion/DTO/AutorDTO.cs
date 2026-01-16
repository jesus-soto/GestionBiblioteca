using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Aplicacion.DTO
{
    public class AutorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}