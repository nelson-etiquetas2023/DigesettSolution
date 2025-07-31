using Digesett.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Digesett.Server.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetDataEmployeeBioAdmin();
        Task<List<Employee>> GetDataEmployee();
        Task ActualizarStatus(List<Employee> ListaException);
        Task<List<Employee>> ExceptionVerifyOut();
        Task<List<Employee>> ActualizarPeriodos(List<Employee> lista);
        Task<List<Employee>> VerificarEmployeeNewBioAdmin(List<Employee> empleados);
        Task<List<Employee>> CheckEmpleadosCancelados(List<Employee> lista_nomina);
        Task<List<PoncheRegistro>> ObtenerHorarioEmpleado(List<PoncheRegistro> ponches); 
    }
}
