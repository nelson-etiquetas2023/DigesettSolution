using Digesett.Shared.Models;


namespace Digesett.Frontend.Services
{
    public interface IEmployeeService
    {
        List<Employee> Employees { get; set; }
        Task<List<Employee>> GetEmployees();
        Task<bool> EmpleadosStatuslicencia(List<Employee> empleados);
        Task VerificarPeriodosVencidos();
        Task ActualizarPeriodos(List<Employee> lista);
        Task<bool> VerificarEmployeeNewBioAdmin(List<Employee> empleados);
        Task<List<PoncheRegistro>> GetShiftsEmployees(List<PoncheRegistro> ponches);
    }
}
