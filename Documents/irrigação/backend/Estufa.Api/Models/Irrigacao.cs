using System.ComponentModel.DataAnnotations;

namespace Estufa.Api.Models
{
    public class Irrigacao
    {
        [Key]
        public int Id { get; set; }

        public DateTime DataHora { get; set; } = DateTime.UtcNow;
        public TimeSpan? Duracao { get; set; }
        public string Status { get; set; } = "Executado"; // Executado, Cancelado, Agendado
        public string? Observacoes { get; set; }
    }
}
