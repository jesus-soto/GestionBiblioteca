using FluentValidation;
using GestionBiblioteca.Aplicacion.DTO;

namespace GestionBiblioteca.Aplicacion.Validadores
{
    public class CrearLibroValidador: AbstractValidator<CrearLibroDTO>
    {
        public CrearLibroValidador()
        {
            RuleFor(x => x.Isbn)
                .NotEmpty().WithMessage("El ISBN es obligatorio")
                .MinimumLength(10).WithMessage("El ISBN debe tener al menos 10 caracteres")
                .MaximumLength(13).WithMessage("El ISBN no puede exceder 13 caracteres");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es obligatorio")
                .MinimumLength(1).WithMessage("El título debe tener al menos 1 carácter")
                .MaximumLength(500).WithMessage("El título no puede exceder 500 caracteres");

            RuleFor(x => x.PublicationYear)
                .GreaterThan(0).WithMessage("El año de publicación debe ser mayor a 0")
                .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("El año de publicación no puede ser futuro");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("El ID del autor es obligatorio");
        }
    }
}