using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estufa.Api.Models
{
    public class Leitura
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Sensor")]
        public int SensorId { get; set; }
        public Sensor? Sensor { get; set; }

        // Valores coletados - nem sempre todos preenchidos dependendo do sensor
        public double? Temperatura { get; set; }
        public double? UmidadeAr { get; set; }
        public double? UmidadeSolo { get; set; }

        public DateTime DataHora { get; set; } = DateTime.UtcNow;
    }
}
