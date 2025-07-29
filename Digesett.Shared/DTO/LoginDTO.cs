using System.ComponentModel.DataAnnotations;

namespace Digesett.Shared.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo electrónico valido")]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        [Display(Name ="Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MinLength(6, ErrorMessage = "El campo {0} debe tener al menos {1} caracteres.")]
        public string Password { get; set; } = null!;
    }
}
