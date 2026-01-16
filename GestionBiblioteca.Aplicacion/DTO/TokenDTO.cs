using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionBiblioteca.Aplicacion.DTO
{
    public class TokenDTO
    {
         public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
    }
}