using Digesett.Shared.Models;

namespace Digesett.Frontend.Services
{
    public interface IServiceCommon
    {
        public Task VerifyEmployeeCanceled(List<Employee> listaNomina); 
    
    }
}
