using System.ComponentModel.DataAnnotations;

namespace Digesett.Shared.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="El Nombre del usuario es requerido")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage ="El email es un campo requerido")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage ="El campo Contraseña es requerido")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "El campo tipo de usuario es requerido")]
        public string TypeUser { get; set; } = null!;
        [Required(ErrorMessage = "El departamento es un campo requerido")]
        public string Departament { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public bool Active { get; set; }
    }
}
