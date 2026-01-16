using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Dominio.Entidades
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string ClaveHash { get; set; } = string.Empty;
    }
}