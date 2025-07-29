using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digesett.Shared.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public string Departamento { get; set; } = null!;
        [Required]
        public string Correo { get; set; } = null!;
        public bool Active { get; set; }
    }
}
