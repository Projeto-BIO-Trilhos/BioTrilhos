namespace BioTrilhos.API.Models;

/// <summary>
/// Tipos de movimentação de água no reservatório.
/// </summary>
public enum TipoMovimentacao
{
    EntradaDeChuva,
    ConsumoIrrigacao,
    AtualizacaoManual,
    Outro
}

/// <summary>
/// Armazena o histórico de alterações de nível dos reservatórios.
/// </summary>
public class HistoricoReservatorio
{
    public int Id { get; set; }
    public int ReservatorioId { get; set; }
    public decimal NivelAnterior { get; set; }
    public decimal NivelAtual { get; set; }
    public TipoMovimentacao TipoMovimentacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Reservatorio? Reservatorio { get; set; }
}
