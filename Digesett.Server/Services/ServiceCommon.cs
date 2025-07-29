using Digesett.Shared.Models;
using Digesett.Server.Services.EmployeeService;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Digesett.Server.Services
{
    public class ServiceCommon : IServiceCommon
    {
        public IConfiguration Configuration { get; set; }
        public string StringConnex { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
        public IEmployeeService ServiceEmployee { get; set; }


        public ServiceCommon(IConfiguration configuration, IEmployeeService serviceEmployee)
        {
            Configuration = configuration;
            StringConnex = Configuration.GetSection("ConnectionStrings").GetSection("CONEX-ASISTENCIA").Value!;
            ServiceEmployee = serviceEmployee;
        }

        public async Task VerificarEmpleadosCancelados(List<Employee> listNomina)
        {
            List<Employee> listaEmployeeCanceled = [];
            List<Employee> listaBioAdmin = await ServiceEmployee.GetDataEmployeeBioAdmin();

            //Selecciona los trabajadores que estan regitrados en BioAdmin
            //que no aparecen en el listado de nomina.

            //listaEmployeeCanceled = [.. listaBioAdmin.Where(p => !listNomina.Any(x => 
            //x.cod_empleado 
            //== p.cod_empleado))];

            listaEmployeeCanceled = listaBioAdmin.Where(p1 => !listNomina.Any(p2 => p2.cod_empleado == p1.cod_empleado)).ToList();



            //cancela todos los trabajadores que no aparecen en el listado de nomina y si el bioadmin
            foreach (var item in listaEmployeeCanceled) 
            {
                await CanceledEmployeeBioAdmin(item);
                Console.WriteLine(item.cod_empleado + " " + item.nombre_empleado);
            }
        }

        public async Task CanceledEmployeeBioAdmin(Employee empleado) 
        {
            var ComentarioFichaUser = "Este Empleado se cancelo desde el Sistema de Nomina. " + DateTime.Now;

            //Procedimiento para cancelar empleados en bioadmin.
            try
            {
                SqlConnection conn = new(StringConnex);
                SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "UPDATE dbo.[User] SET active=0,comment=@p2 WHERE iduser=@p1"
                };
                SqlParameter p1 = new("@p1", empleado.cod_empleado);
                SqlParameter p2 = new("@p2", ComentarioFichaUser);
                comando.Parameters.Add(p1);
                comando.Parameters.Add(p2);
                await conn.OpenAsync();
                await comando.ExecuteNonQueryAsync();
                Console.WriteLine("Se Ejecuto el comando de Cancelación de empleados correctamente... [Iduser:]" + empleado.cod_empleado);   
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                ErrorMessage = ex.Message; 
            }
        }
    }
}
