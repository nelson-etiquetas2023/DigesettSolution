using Digesett.Shared.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Digesett.Frontend.Services
{
    public class ServiceCommon(IHttpClientFactory httpClientFactory, IJSRuntime JS) : IServiceCommon
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        public  IJSRuntime Js { get; } = JS;
        private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };
       

        public async Task VerifyEmployeeCanceled(List<Employee> listaNomina)
        { 
            var json = JsonSerializer.Serialize(listaNomina, jsonOptions);
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
            
            

            var cliente = _httpClientFactory.CreateClient("Digesett");
            var url = $"/api/Common/Verificar-Empleados-Cancelados";

            var response = await cliente.PostAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var empleadosCancelados = await response.Content.ReadFromJsonAsync<List<Employee>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                await Js.InvokeVoidAsync("mostrarEmpleadosEnDialogo", empleadosCancelados);
            }
            else 
            {
                Console.WriteLine($"Error: {response.StatusCode}");   
            }
        }
    }
}
