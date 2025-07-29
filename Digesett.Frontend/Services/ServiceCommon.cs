using Digesett.Shared.Models;
using System.Text;
using System.Text.Json;

namespace Digesett.Frontend.Services
{
    public class ServiceCommon(IHttpClientFactory httpClientFactory) : IServiceCommon
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task VerifyEmployeeCanceled(List<Employee> listaNomina)
        {
            var json = JsonSerializer.Serialize(listaNomina, jsonOptions);
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            var cliente = _httpClientFactory.CreateClient("Digesett");
            var url = $"/api/Common/EmpleadosCancelados";

            var response = await cliente.PostAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Solicitud realizada con Exito!!!");
            }
            else 
            {
                Console.WriteLine($"Error: {response.StatusCode}");   
            }
        }
    }
}
