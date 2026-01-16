using FluentValidation;
using GestionBiblioteca.Aplicacion.DTO;

namespace GestionBiblioteca.Aplicacion.Validadores
{
    public class LoginValidador : AbstractValidator<LoginDTO>
    {
        public LoginValidador()
        {
            RuleFor(x => x.NombreUsuario)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio");

            RuleFor(x => x.Clave)
                .NotEmpty().WithMessage("La contraseña es obligatoria");
        }
    }
}