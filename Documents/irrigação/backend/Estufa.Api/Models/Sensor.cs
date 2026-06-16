using System.ComponentModel.DataAnnotations;

namespace Estufa.Api.Models
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; } = null!;
        public string Tipo { get; set; } = null!; // e.g., "DHT22", "Capacitivo"
        public string Localizacao { get; set; } = "Estufa";
        public string Unidade { get; set; } = "%";
        public bool Ativo { get; set; } = true;

        public ICollection<Leitura> Leituras { get; set; } = new List<Leitura>();
    }
}
