using System.ComponentModel.DataAnnotations;

namespace Estufa.Api.Models
{
    public class Reservatorio
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = "Reservatorio Principal";
        public double Nivel { get; set; } // percentual 0-100
        public double CapacidadeLitros { get; set; }
        public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
