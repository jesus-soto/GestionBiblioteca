using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Aplicacion.DTO
{
    public class ActualizarLibroDTO
    {
        public string? Title { get; set; } = string.Empty;
        public int? PublicationYear { get; set; }
        public Guid? AuthorId { get; set; }
    }
}