using CurrieTechnologies.Razor.SweetAlert2;
using Digesett.Shared.Models;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace Digesett.Frontend.Services
{
    public class EmployeeService(HttpClient httpClient, SweetAlertService swal, IJSRuntime JS) : IEmployeeService
    {
        public HttpClient HttpClient { get; set; } = httpClient;
        public List<Employee> Employees { get; set; } = null!;
        SweetAlertService Swal { get; set; } = swal;
        public IJSRuntime Js { get; } = JS;
        private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<List<Employee>> GetEmployees()
        {
            var url = $"http://172.16.0.18:2030/Api/Employee";
            var respuesta = await HttpClient.GetAsync(url);
            var respuestaString = await respuesta.Content.ReadAsStringAsync();
            Employees = JsonSerializer.Deserialize<List<Employee>>(respuestaString,jsonOptions)!;

            return Employees;
        }
        public async Task<bool> ActualizarStatusNomina(List<Employee> empleados)
        {
            //Consulta de los Empleados que estan de Exception o en 
            //status fuera de nomina.
            var queryEx = (from q in empleados
                           where q.FueraNomina
                           select q).ToList();

           
            //endpoint que procesa la peticion de actualizar bioadmin.
            var url = $"http://172.16.0.18:2030/api/Employee/ActualizarStatus";

            //hacemos la peticion al secver
            var httpclient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(queryEx, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var respuesta = await HttpClient.PostAsync(url, content);
            if (respuesta.IsSuccessStatusCode)
            {
                await Swal.FireAsync("advertencia", "Tarea1: Actualizar bioadmin status fuera nomina", SweetAlertIcon.Success);
            }
            else
            {
                await Swal.FireAsync("Error", respuesta.Content.ReadAsStringAsync().ToString(), SweetAlertIcon.Error);
            }
            return respuesta.IsSuccessStatusCode;
        }
        public async Task VerificarPeriodosVencidos()
        {
            var url = @"http://172.16.0.18:2030/api/employee/ExceptionVerifyOut";
            var respuesta = await HttpClient.GetAsync(url);
            var rptaString = await respuesta.Content.ReadAsStringAsync();
            var lista = JsonSerializer.Deserialize<List<Employee>>(rptaString,jsonOptions);
            //ejecucioon de javascript para mostrar la table con los empleados
            //con periodos vencidos
            await Js.InvokeAsync<object>("ListaEmpleadosExcepctionOut", lista);
        }
        public async Task ActualizarPeriodos(List<Employee> lista)
        {
            //Actualizar los empleados con periodos vencidos con la lista desde javascript.
            //la lista se llama ListNominaIn.
            //opciones del json.
            foreach (var item in lista)
            {
                item.cargo = "x";
                item.cedula = "1";
                item.departamento = "sistema";
                item.IdDepart = "1";
                item.nombre_empleado = "x";
                item.status = "reingreso nomina";

            }
            //endpoint para actualizar el bioadmin y reingresar las personas que se le vencio el periodo
            var url = $"http://172.16.0.18:2030/api/Employee/actualizarPeriodos";
            //hacemos la peticion al server.
            var httpclient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(lista, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var respuesta = await httpclient.PostAsync(url, content);
            if (respuesta.IsSuccessStatusCode)
            {
                Console.WriteLine("Peticion realizada con exito!!!");
            }
            else 
            {
                Console.WriteLine($"Error: {respuesta.StatusCode}");
            }
        }
        public async Task<bool> VerificarEmployeeNewBioAdmin(List<Employee> empleados)
        {
            
            var url = $"http://172.16.0.18:2030/api/Employee/VerificarEmployeeNewBioAdmin";
            var httpclient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(empleados, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var respuesta = await httpclient.PostAsync(url, content);
            if (respuesta.IsSuccessStatusCode)
            {
                await Swal.FireAsync("Advertencia", "Ejecucion de la Tarea de Buscar Empleados Nuevos realizada con Exito!", SweetAlertIcon.Success);
            }
            else
            {
                await Swal.FireAsync("Error", respuesta.Content.ReadAsStringAsync().ToString(), SweetAlertIcon.Error);
            }
            return respuesta.IsSuccessStatusCode;
        }

        public async Task<bool> EmpleadosCancelados(List<Employee> nomina)
        {
            nomina.ForEach(x => x.Activo = true);
            nomina.ForEach(x => x.IdDepart = "1");
            //llenar la lista desde bioadmin.
            var url = $"http://172.16.0.18:2030/api/employee/CheckEmpleadosCancelados";
            var httpclient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(nomina, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var respuesta = await HttpClient.PostAsync(url, content);
            if (respuesta.IsSuccessStatusCode)
            {
                var responseBody = await respuesta.Content.ReadAsStringAsync();
                //listaEmpCancelados = JsonSerializer.Deserialize<List<Employee>>(responseBody)!;
            }
            return true;
        }

        public async Task<List<PoncheRegistro>> GetShiftsEmployees(List<PoncheRegistro> ponches)
        {
            //Establecer valores por defecto porque sino responde 400
            ponches.ForEach(x => x.ShiftName="Sin Asignar.");
            ponches.ForEach(x => x.ShiftStart = 0);
            ponches.ForEach(x => x.ShiftEnd = 0);
            var url = $"http://172.16.0.18:2030/api/employee/GetHorariosEmpleados";
            var jsonContent = JsonSerializer.Serialize(ponches, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var httpclient = new HttpClient();
            var respuesta = await httpclient.PostAsync(url,content);
            if (respuesta.IsSuccessStatusCode)
            {
                var rptastring = await respuesta.Content.ReadAsStringAsync();
                var listaUpdate = JsonSerializer.Deserialize<List<PoncheRegistro>>(rptastring,jsonOptions);
                return listaUpdate!;
            }
            else
            {
                return [];
            }
        }
    }
}
