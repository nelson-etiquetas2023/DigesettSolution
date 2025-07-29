using System.ComponentModel.DataAnnotations;

namespace Digesett.Shared.Models
{
    public class ActivoFijo
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Category { get; set; } = null!;
        public bool Active { get; set; }

    }
}
