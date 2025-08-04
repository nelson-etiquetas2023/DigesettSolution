using Digesett.Shared.Models;

namespace Digesett.Server.Services
{
    public interface IServiceCommon
    {
        Task<List<Employee>> EmpleadosCancelados(List<Employee> lista);    
    }
}
