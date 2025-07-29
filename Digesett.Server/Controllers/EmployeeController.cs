using Digesett.Server.Services.EmployeeService;
using Digesett.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Digesett.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(IEmployeeService employeeService) : ControllerBase
    {
        public IEmployeeService EmployeeService { get; set; } = employeeService;

        [HttpPost("/api/employee/GetHorariosEmpleados")]
        public async Task<ActionResult<List<PoncheRegistro>>> GetHorariosEmpleados([FromBody] List<PoncheRegistro> listaponches) 
        {
            return await EmployeeService.ObtenerHorarioEmpleado(listaponches);
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetDataEmployee() 
        {
            List<Employee> result = await EmployeeService.GetDataEmployee();
            return Ok(result);
        }

        [HttpPost("/api/Employee/ActualizarStatus")]
        public async Task ActualizarStatus(List<Employee> ListaException)
        {
            await EmployeeService.ActualizarStatus(ListaException);
        }   

        //verifica en la nomina de bioadmin los empleados que estan fuera de nomina que se le haya cumplido el periodo.
        [HttpGet("/api/employee/ExceptionVerifyOut")]                
        public async Task<ActionResult<ServiceResponse<List<Employee>>>> ExceptionVerifyOut() 
        {
            var result = await EmployeeService.ExceptionVerifyOut();
            return Ok(result);
        }

        [HttpPost("/api/employee/ActualizarPeriodos")]
        public async Task<ActionResult<bool>> ActualizarPeriodos ([FromBody] List<Employee> listaEmp) 
        {
            var result = await EmployeeService.ActualizarPeriodos(listaEmp);
            return Ok(result);
        }

        [HttpPost("/api/employee/VerificarEmployeeNewBioAdmin")]
        public async Task<ActionResult<bool>> VerificarEmployeeNewBioAdmin(List<Employee> empleados) 
        {
            var result = await EmployeeService.VerificarEmployeeNewBioAdmin(empleados);
            return Ok(result);
        }

        [HttpPost("/api/employee/CheckEmpleadosCancelados")]
        public async Task<ActionResult<List<Employee>>> CheckEmpleadosCancelados(List<Employee> empleados) 
        {
            List<Employee> result = await EmployeeService.CheckEmpleadosCancelados(empleados);
            return Ok(result);
        }




    }
}

