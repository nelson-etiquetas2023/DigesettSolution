namespace Digesett.Shared.Models
{
    public class ParametroShift
    {
        public Int32 ShiftId { get; set; } 
        public string ShiftName { get; set; } = null!;
        public Int32  IndexDay { get; set; } 
        public string StringDay { get; set; } = null!;
        public Int32 Entrada { get; set; }
        public Int32 Salida { get; set; }
    }
}
