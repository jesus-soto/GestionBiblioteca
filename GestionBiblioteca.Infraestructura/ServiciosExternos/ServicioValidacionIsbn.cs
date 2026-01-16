using System;
using System.ServiceModel;
using System.Threading.Tasks;
using GestionBiblioteca.Aplicacion.Interfaces;

namespace GestionBiblioteca.Infraestructura.ServiciosExternos
{
    public class ServicioValidacionIsbn : IServicioValidacionIsbn
    {
        public async Task<bool> ValidarIsbnAsync(string isbn)
        {
            try
            {
                var binding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = 2147483647,
                    Security = new BasicHttpSecurity
                    {
                        Mode = BasicHttpSecurityMode.Transport
                    }
                };

                var endpoint = new EndpointAddress("https://webservices.daehosting.com/services/isbnservice.wso");
                
                var factory = new ChannelFactory<IISBNService>(binding, endpoint);
                var channel = factory.CreateChannel();

                // Intentar validar el ISBN usando el servicio SOAP
                var resultado = await Task.Run(() => 
                {
                    try
                    {
                        // El servicio retorna información del libro si el ISBN es válido
                        var respuesta = channel.GetBookInfo(isbn);
                        return !string.IsNullOrEmpty(respuesta);
                    }
                    catch
                    {
                        return false;
                    }
                });

                ((IClientChannel)channel).Close();
                factory.Close();

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar ISBN {isbn}: {ex.Message}");
                // En caso de error del servicio, validar formato básico
                return ValidarFormatoIsbn(isbn);
            }
        }

        private bool ValidarFormatoIsbn(string isbn)
        {
            // Eliminar guiones y espacios
            isbn = isbn.Replace("-", "").Replace(" ", "");

            // Debe ser de 10 o 13 dígitos
            if (isbn.Length != 10 && isbn.Length != 13)
                return false;

            // Validar que todos los caracteres sean dígitos (excepto el último de ISBN-10 que puede ser X)
            for (int i = 0; i < isbn.Length; i++)
            {
                if (i == isbn.Length - 1 && isbn.Length == 10 && isbn[i] == 'X')
                    continue;
                
                if (!char.IsDigit(isbn[i]))
                    return false;
            }

            return true;
        }
    }

    // Contrato del servicio SOAP
    [ServiceContract]
    public interface IISBNService
    {
        [OperationContract]
        string GetBookInfo(string isbn);
    }
}