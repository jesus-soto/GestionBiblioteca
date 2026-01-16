using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Aplicacion.DTO
{
    public class LibroDTO
    {
        public Guid Id { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
    }
}