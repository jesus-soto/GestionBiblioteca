using System.Text;
using System.Text.RegularExpressions;
using GestionBiblioteca.Aplicacion.Interfaces;

namespace GestionBiblioteca.Aplicacion.Servicios
{
    public class ServicioNormalizacion : IServicioNormalizacion
    {
        public string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            // Convertir a mayúsculas
            texto = texto.ToUpperInvariant();

            // Eliminar números
            texto = Regex.Replace(texto, @"\d", "");

            // Reemplazar caracteres especiales por sus equivalentes simples
            texto = EliminarDiacriticos(texto);

            // Reemplazar múltiples espacios por uno solo
            texto = Regex.Replace(texto, @"\s+", " ");

            // Eliminar espacios al inicio y final
            texto = texto.Trim();

            return texto;
        }

        private string EliminarDiacriticos(string texto)
        {
            var normalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizado)
            {
                var categoriaUnicode = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoriaUnicode != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}