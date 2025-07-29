using System.ComponentModel.DataAnnotations;

namespace Digesett.Shared.Models
{
    public class Employee
    {
        [Required]
        public string cod_empleado { get; set; } = null!;
        public string cedula { get; set; } = null!;
        public string nombre_empleado { get; set; } = null!;
        public string IdDepart { get; set; } = null!;
        public string departamento { get; set; } = null!;
        public string cargo { get; set; } = null!;
        public string status { get; set; } = null!;
        public bool FueraNomina { get; set; } = false;
        public DateTime ExceptionDateStart { get; set; } 
        public DateTime ExceptionDateEnd { get; set; }
        public bool Activo { get; set; } = false;
    }
}
