using Digesett.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Digesett.Server.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly List<Employee> lista = [];
       
        
        public string errorConn = "";
        public bool errorStatus = false;
        public IConfiguration Configuration { get; set; }
        public string Strconn { get; set; }
        public string StrConnBioAdmin { get; set; } = null!;
       
        public EmployeeService(IConfiguration configuration)
        {
            Configuration = configuration;
            Strconn = Configuration.GetSection("ConnectionStrings").GetSection("CONEX-NOMINA").Value!;
            StrConnBioAdmin = Configuration.GetSection("ConnectionStrings").GetSection("CONEX-ASISTENCIA").Value!;
        }

        public async Task<List<Employee>> GetDataEmployeeBioAdmin() 
        {
            try
            {
                using SqlConnection conn = new(StrConnBioAdmin);
                SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT iduser,name FROM [dbo].[User]"
                };
                await conn.OpenAsync();
                SqlDataReader reader = comando.ExecuteReader();
                while (await reader.ReadAsync())
                {
                    //creo una instancia del objeto punchdayly para incluirlo en la lista.
                    Employee emp = new()
                    {
                        cod_empleado = Convert.ToString(reader.GetInt32(0)),
                        nombre_empleado = reader.GetString(1)
                    };
                    lista.Add(emp);
                }
                await reader.CloseAsync();
            }
            catch (SqlException ex)
            {
                errorConn = ex.Message;
            }
            return lista;
        } 

        public async Task<List<Employee>> GetDataEmployee()
        {
            
            //contruir el objeto conexion de SQL a la base de datos.
            using SqlConnection conn = new(Strconn);
            //construir el objeto comando SQL.
            SqlCommand comando = new("Usp_Select_Info_Reloj", conn)
            {
                 CommandType = CommandType.StoredProcedure
            };
            //abrir la base de datos.
            await conn.OpenAsync();
            //Ejecutar el reader que hace la consulta y devuelve un SqlDataReader.
            SqlDataReader reader = comando.ExecuteReader();
            while (await reader.ReadAsync())
            {
                //creo una instancia del objeto punchdayly para incluirlo en la lista.
                Employee emp = new()
                {
                    cod_empleado = reader.GetString("codigoempleado").TrimStart('0'),
                    cedula = reader.GetString("cedula"),
                    nombre_empleado = reader.GetString("NombreEmpleado"),
                    departamento = reader.GetString("departamento"),
                    cargo = reader.GetString("cargo"),
                    status = reader.GetString("estatus"),
                    FueraNomina = reader.GetBoolean("FueraNomina"),
                    ExceptionDateStart = reader.GetDateTime("ExFechaDesde"),
                    ExceptionDateEnd = reader.GetDateTime("ExFechaHasta"),
                    IdDepart = "0"
                };
                lista.Add(emp);
            }
            await reader.CloseAsync();
            return lista;
        }
        public async Task ActualizarStatus(List<Employee> ListaException)
        {
            try
            {

                using SqlConnection conn = new(StrConnBioAdmin);
                await conn.OpenAsync();
                foreach (var item in ListaException)
                {
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "update dbo.[User] set active=@p2,ApplyExceptionPermition=@p3," +
                                      "ExceptionPermitionBegin=@p4,ExceptionPermitionEnd=@p5 where IdentificationNumber=@p1"
                    };
                    SqlParameter p1 = new("@p1", item.cedula);
                    SqlParameter p2 = new("@p2", false);
                    SqlParameter p3 = new("@p3", true);
                    SqlParameter p4 = new("@p4", item.ExceptionDateStart);
                    SqlParameter p5 = new("@p5", item.ExceptionDateEnd);
                    comando.Parameters.Add(p1);
                    comando.Parameters.Add(p2);
                    comando.Parameters.Add(p3);
                    comando.Parameters.Add(p4);
                    comando.Parameters.Add(p5);
                    await comando.ExecuteNonQueryAsync();
                };
                await conn.CloseAsync();

            }
            catch (SqlException ex)
            {
                //controla si hay error.
                Console.WriteLine(ex.Message);

            }
        }
        public async Task<List<Employee>> ExceptionVerifyOut()
        {
            //verifica si existen empleados en bioadmin que se le haya vencido el 
            //perido de excepcion.
            using SqlConnection conn = new(StrConnBioAdmin);
            SqlCommand comando = new("SP_LIST_EXCEPTION_OFF", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            conn.Open();
            SqlDataReader reader = await comando.ExecuteReaderAsync();
            while (reader.Read())
            {
                //creo una instancia del objeto punchdayly para incluirlo en la lista.
                Employee emp = new()
                {
                    Activo = reader.GetBoolean("Active"),
                    cedula = reader.GetString("IdentificationNumber"),
                    cod_empleado = Convert.ToString(reader.GetInt32("IdUser")),
                    nombre_empleado = reader.GetString("name"),
                    IdDepart = Convert.ToString(reader.GetInt32("IdDepartment")),
                    departamento = reader.GetString("departamento"),
                    cargo = reader.GetString("position"),
                    ExceptionDateStart = reader.GetDateTime("ExceptionPermitionBegin"),
                    ExceptionDateEnd = reader.GetDateTime("ExceptionPermitionEnd")
                };
                lista.Add(emp);
            }
            reader.Close();
            return lista;
        }
        public async Task<List<Employee>> ActualizarPeriodos(List<Employee> listaEmp)
        {
            using SqlConnection conn = new(StrConnBioAdmin);
            await conn.OpenAsync();
            foreach (var item in listaEmp)
            {
                SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "update dbo.[User] set active=@p2,ApplyExceptionPermition=@p3," +
                                      "ExceptionPermitionBegin=@p4,ExceptionPermitionEnd=@p5 where IdUser=@p1"
                };
                SqlParameter p1 = new("@p1", item.cod_empleado);
                SqlParameter p2 = new("@p2", true);
                SqlParameter p3 = new("@p3", false);
                SqlParameter p4 = new("@p4", DateTime.Today);
                SqlParameter p5 = new("@p5", DateTime.Today);
                comando.Parameters.Add(p1);
                comando.Parameters.Add(p2);
                comando.Parameters.Add(p3);
                comando.Parameters.Add(p4);
                comando.Parameters.Add(p5);
                await comando.ExecuteNonQueryAsync();
            };
            await conn.CloseAsync();
            return lista;
        }
        public async Task<List<Employee>> VerificarEmployeeNewBioAdmin(List<Employee> empleados)
        {
            //filtra los empleado que estan en nomina.
            var listaenNomina = empleados.Where(x => x.FueraNomina == false).ToList();
            var listaEmplenuevos = new List<Employee>();
            try
            { 
                SqlConnection conn = new(StrConnBioAdmin);
                await conn.OpenAsync();
                //recorrer para verificar en bio-admin.
                foreach (var item in listaenNomina)
                {
                    //primera consulta para verificar que existe o no el empleado en el bioadmin
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT count(*) from [BDBioAdminSQL].[dbo].[user] where iduser=@p1",
                    };
                    SqlParameter p1 = new("@p1", item.cod_empleado);
                    comando.Parameters.Add(p1);
                    object result = comando.ExecuteScalar();
                    //verificacion del empleado
                    if (result != null && (int)result > 0)
                    {
                        // el empleado existe y lo activo en el bio-admin.
                        SqlCommand comandoExiste = new()
                        {
                            Connection = conn,
                            CommandType = CommandType.Text,
                            CommandText = "UPDATE [dbo].[User] set active=1 where IdUser=@t1"
                        };
                        SqlParameter t1 = new("@t1", item.cod_empleado);
                        comandoExiste.Parameters.Add(t1);
                        comandoExiste.ExecuteNonQuery();
                    }
                    else
                    {
                        string InsertSql = "INSERT INTO[dbo].[User] (iduser, name, IdDepartment, Active, Privilege," +
                                            "HourSalary,PreferredIdLanguage, CreatedBy, CreatedDatetime, ModifiedBy," +
                                            "ModifiedDatetime, UseShift) VALUES(@c1,@c2, 1, 1, 0, 0, 0," +
                                            "CONVERT(varchar, GETDATE(), 112),CONVERT(varchar, GETDATE(), 112)," +
                                            "CONVERT(varchar, GETDATE(), 112), CONVERT(varchar, GETDATE(), 112), 1)";

                        //el empleado no existe en bio-admin. Hay que crearlo porque es nuevo
                        SqlCommand comandoNoExite = new()
                        {
                            Connection = conn,
                            CommandType = CommandType.Text,
                            CommandText = InsertSql
                        };
                        SqlParameter c1 = new("@c1", item.cod_empleado);
                        SqlParameter c2 = new("@c2", item.nombre_empleado);
                        SqlParameter c3 = new("@c3", item.cedula);
                        SqlParameter c4 = new("@c4", item.Activo);
                        SqlParameter c5 = new("@c5", item.IdDepart);
                        SqlParameter c6 = new("@c6", item.departamento);
                        SqlParameter c7 = new("@c7", item.cargo);
                        comandoNoExite.Parameters.Add(c1);
                        comandoNoExite.Parameters.Add(c2);
                        comandoNoExite.Parameters.Add(c3);
                        comandoNoExite.Parameters.Add(c4);
                        comandoNoExite.Parameters.Add(c5);
                        comandoNoExite.Parameters.Add(c6);
                        comandoNoExite.Parameters.Add(c7);
                        comandoNoExite.ExecuteNonQuery();
                        //agrego el empleado a la lista de nuevo.
                        Employee Nuevo = new Employee()
                        {
                            cod_empleado = item.cod_empleado,
                            nombre_empleado = item.nombre_empleado,
                            departamento = item.departamento,
                        };
                        listaEmplenuevos.Add(Nuevo);
                    }

                }
            }
            catch(SqlException ex)
            {
                errorConn = ex.Message;
            }
            return listaEmplenuevos;
        }

     
        public async Task<List<Employee>> CheckEmpleadosCancelados(List<Employee> Lista_Nomina)
        {
            //Crear la lista con los empleado de bio-admin.
            List<Employee> ListaBio = [];
            using (SqlConnection conn = new(StrConnBioAdmin)) 
            {
                SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "select a.iduser,b.Name from record a left join [dbo].[User] b on a.IdUser = " +
                    "b.IdUser"
                };
                await conn.OpenAsync();
                using SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Employee emplo = new()
                    {
                        cod_empleado = reader.GetString("iduser"),
                        nombre_empleado = reader.GetString("name")
                    };
                    ListaBio.Add(emplo);
                }
            }
            return ListaBio;

               
            
           

                   
        }

        public async Task<List<PoncheRegistro>> ObtenerHorarioEmpleado(List<PoncheRegistro> ponches)
        {
            //recorro la lista cruda y voy buscando los datos de los horarios para irlos asignando
            foreach (var item in ponches) 
            {
                using SqlConnection conn = new(StrConnBioAdmin);
                //obtener el id del horario del empleado
                SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT ShiftId FROM [BDBioAdminSQL].[dbo].[UserShift] WHERE IdUser=@p1"
                };
                SqlParameter p1 = new("@p1", item.Id);
                comando.Parameters.Add(p1);
                await conn.OpenAsync();
                int ShiftId = Convert.ToInt16(comando.ExecuteScalar());
                //buscar los parametros del horario.
                ParametroShift ParamShiftDay = new();
                SqlCommand comparams = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT a.ShiftId,b.Description as Horario,a.DayId as IndexDay,dia_letra=CASE WHEN DayId = 0 then 'LUNES' " +
                    "WHEN DayId = 1 THEN 'MARTES' WHEN DayId = 2 THEN 'MIERCOLES' WHEN DayId = 3 THEN 'JUEVES' " +
                    "WHEN DayId = 4 then 'VIERNES' WHEN DayId = 5 THEN 'SABADO' WHEN DayId = 6 then 'DOMINGO' END, " +
                    "a.t2inhour as ENTRADA,a.t2outhour as SALIDA " +
                    "FROM ShiftDetail a LEFT JOIN Shift b ON b.ShiftId = a.ShiftId " +
                    "WHERE a.ShiftId = @x1 and a.DayId = @x2"
                };
                SqlParameter x1 = new("@x1", ShiftId);
                SqlParameter x2 = new("@x2", item.Indexday);
                comparams.Parameters.Add(x1);
                comparams.Parameters.Add(x2);
                //ejecutar el reader.
                try
                {
                    SqlDataReader reader = await comparams.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    item.ShiftName = reader.GetString(1);
                    item.ShiftStart = reader.GetInt32(4);
                    item.ShiftEnd = reader.GetInt32(5);
                }
                catch (Exception ex)
                {
                    var megssage = ex.Message;
                }
                conn.Close();
            }
            return ponches;
        }
    }
}
