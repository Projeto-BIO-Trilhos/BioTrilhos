namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para Irrigação.
/// </summary>
public class IrrigacaoDto
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public int? ReservatorioId { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime? DataHoraFim { get; set; }
    public int DuracaoSegundos { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public int TipoAcionamento { get; set; }
    public int Status { get; set; }
}

/// <summary>
/// DTO para iniciar/parar irrigação.
/// </summary>
public class AcionarIrrigacaoDto
{
    public int EstufaId { get; set; }
    public int? ReservatorioId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public int? DuracaoSegundos { get; set; }
    public int TipoAcionamento { get; set; } = 1; // Manual
}
