using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Digesett.Shared.Models;
using System.Data;

namespace Digesett.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public SqlConnection Conn { get; set; } = null!;
        public string StringConex { get; set; } = string.Empty;
        public string ErrorConn { get; set; } = null!;
        public bool ErrorStatus  { get; set; }
        readonly List<PoncheRegistro> Lista = [];

        public AttendanceController(IConfiguration configuration)
        {
            Configuration = configuration;
            StringConex = Configuration.GetSection("ConnectionStrings").GetSection("CONEX-ASISTENCIA").Value!;
        }
        
        [HttpGet]
        public async Task<List<PoncheRegistro>> GetDataPonches([FromQuery] DateTime startDate,[FromQuery] DateTime endDate) 
        {
            try
            {
                using SqlConnection conn = new(StringConex);
                SqlCommand comando = new("SP_LOAD_PONCHES_BIOADMIN", conn) 
                {
                    CommandType = CommandType.StoredProcedure
                };
                comando.Parameters.AddWithValue("@tdesde", startDate);
                comando.Parameters.AddWithValue("@thasta", endDate);
                await conn.OpenAsync();
                SqlDataReader reader = await comando.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    PoncheRegistro ponche = new()
                    {
                        Id = reader.GetInt32("Iduser"),
                        Empleado = reader.GetString("empleado"),
                        RecordTime = reader.GetDateTime("fecha"),
                        Indexday = reader.GetInt32("indexday"),
                        StringNameDate = reader.GetString("dia"),
                        Marca1 = reader.GetString("m1"),
                        Marca2 = reader.GetString("m2"),
                        Marca3 = reader.GetString("m3"),
                        Marca4 = reader.GetString("m4"),
                        NumbersPonches = reader.GetInt64("ponches"),
                        Departamento = reader.GetString("departamento"),
                        Cargo =  reader.GetString("cargo") != null ? reader.GetString("cargo") : ""
                    };
                    Lista.Add(ponche);
                }
                await reader.CloseAsync();
            }
            catch (SqlException ex) 
            {
                ErrorConn = ex.Message;
                ErrorStatus = true;
            }
            return Lista;
        }
    }
}
