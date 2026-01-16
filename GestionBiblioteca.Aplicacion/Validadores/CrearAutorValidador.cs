using FluentValidation;
using GestionBiblioteca.Aplicacion.DTO;

namespace GestionBiblioteca.Aplicacion.Validadores
{
    public class CrearAutorValidador: AbstractValidator<CrearAutorDTO>
    {
        public CrearAutorValidador()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");
        }
    }
}