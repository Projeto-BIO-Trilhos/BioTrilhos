namespace BioTrilhos.API.Models;

/// <summary>
/// Representa um reservatório de água na estufa.
/// </summary>
public class Reservatorio
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal CapacidadeMaxima { get; set; }
    public decimal NivelAtual { get; set; }
    public decimal PercentualAtual { get; set; }
    public string Status { get; set; } = "Normal";
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Estufa? Estufa { get; set; }
    public virtual ICollection<HistoricoReservatorio> Historicos { get; set; } = new List<HistoricoReservatorio>();
    public virtual ICollection<Irrigacao> Irrigacoes { get; set; } = new List<Irrigacao>();
}
