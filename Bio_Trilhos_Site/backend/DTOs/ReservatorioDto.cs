namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para Reservatório.
/// </summary>
public class ReservatorioDto
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal CapacidadeMaxima { get; set; }
    public decimal NivelAtual { get; set; }
    public decimal PercentualAtual { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataUltimaAtualizacao { get; set; }
}

/// <summary>
/// DTO para criar/atualizar Reservatório.
/// </summary>
public class CreateReservatorioDto
{
    public int EstufaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal CapacidadeMaxima { get; set; }
    public decimal NivelAtual { get; set; }
}
