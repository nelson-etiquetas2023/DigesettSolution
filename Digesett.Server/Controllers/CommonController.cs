using Digesett.Server.Services;
using Digesett.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Digesett.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        public IServiceCommon ServiceCommon { get; set; }

        public CommonController(IServiceCommon serviceCommon)
        {
            ServiceCommon = serviceCommon;
        }

        [HttpPost("EmpleadosCancelados")]
        public void VerifyEmployeeCanceled(List<Employee> lista)  
        {
            ServiceCommon.VerificarEmpleadosCancelados(lista);
        }
    }
}
