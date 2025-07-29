using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digesett.Shared.Models
{
    public class Departament
    {
        public int IdDepartament { get; set; }
        public int IdParent {get; set; }
        public string Name { get; set; } = null!;
        public string Comment { get; set; } = null!;
    }
}
