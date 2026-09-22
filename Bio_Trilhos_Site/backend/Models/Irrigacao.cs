namespace BioTrilhos.API.Models;

/// <summary>
/// Tipos de acionamento de irrigação.
/// </summary>
public enum TipoAcionamento
{
    Automatico,
    Manual
}

/// <summary>
/// Status da irrigação.
/// </summary>
public enum StatusIrrigacao
{
    Ativa,
    Finalizada,
    Cancelada,
    Erro
}

/// <summary>
/// Armazena os acionamentos do sistema de irrigação.
/// </summary>
public class Irrigacao
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public int? ReservatorioId { get; set; }
    public DateTime DataHoraInicio { get; set; } = DateTime.UtcNow;
    public DateTime? DataHoraFim { get; set; }
    public int DuracaoSegundos { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public TipoAcionamento TipoAcionamento { get; set; }
    public StatusIrrigacao Status { get; set; } = StatusIrrigacao.Ativa;

    // Relacionamentos
    public virtual Estufa? Estufa { get; set; }
    public virtual Reservatorio? Reservatorio { get; set; }
}
