using Digesett.Shared.Models;

namespace Digesett.Server.Services
{
    public interface IServiceCommon
    {
        Task VerificarEmpleadosCancelados(List<Employee> lista);    
    }
}
