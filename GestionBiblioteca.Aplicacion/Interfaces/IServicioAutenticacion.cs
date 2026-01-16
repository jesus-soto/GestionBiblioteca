using System.Threading.Tasks;
using GestionBiblioteca.Aplicacion.DTO;

namespace GestionBiblioteca.Aplicacion.Interfaces
{
    public interface IServicioAutenticacion
    {
        Task<TokenDTO?> AutenticarAsync(LoginDTO loginDto);
    }
}