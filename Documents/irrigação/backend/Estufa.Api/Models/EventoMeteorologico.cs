using System.ComponentModel.DataAnnotations;

namespace Estufa.Api.Models
{
    public class EventoMeteorologico
    {
        [Key]
        public int Id { get; set; }
        public string Tipo { get; set; } = null!; // Ex: PrevisaoChuva, Alerta
        public string? Descricao { get; set; }
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
        public string? Dados { get; set; } // JSON bruto da API meteorológica
    }
}
