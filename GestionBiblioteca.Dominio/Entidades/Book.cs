using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Dominio.Entidades
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public Guid AuthorId { get; set; }
        public virtual Author? Author { get; set; }    
    }
}