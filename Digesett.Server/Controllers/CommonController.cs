using Digesett.Server.Services;
using Digesett.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Digesett.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController(IServiceCommon serviceCommon) : ControllerBase
    {
        public IServiceCommon ServiceCommon { get; set; } = serviceCommon;



        [HttpPost("Verificar-Empleados-Cancelados")]
        public Task<List<Employee>> PostAsync(List<Employee> lista)  
        {
            return ServiceCommon.EmpleadosCancelados(lista);
            
        }
    }
}
