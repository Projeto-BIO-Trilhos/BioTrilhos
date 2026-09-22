namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para dados de Dashboard.
/// </summary>
public class DashboardDto
{
    public int EstufaId { get; set; }
    public string EstufaNome { get; set; } = string.Empty;
    
    // Sensores - Últimas leituras
    public decimal TemperaturaAtual { get; set; }
    public decimal UmidadeArAtual { get; set; }
    public decimal UmidadeSoloAtual { get; set; }
    public DateTime UltimaAtualizacaoSensores { get; set; }
    
    // Reservatório
    public decimal ReservatorioNivel { get; set; }
    public decimal ReservatorioCapacidade { get; set; }
    public decimal ReservatorioPercentual { get; set; }
    public string ReservatorioStatus { get; set; } = string.Empty;
    
    // Irrigação
    public bool IrrigacaoAtiva { get; set; }
    public DateTime? UltimaIrrigacao { get; set; }
    public int DuracaoUltimaIrrigacao { get; set; }
    
    // Meteorologia
    public decimal TemperaturaExterna { get; set; }
    public decimal UmidadeExterna { get; set; }
    public string CondicaoClimatica { get; set; } = string.Empty;
    public bool EstaChovendo { get; set; }
    public decimal? ProbabilidadeChuva { get; set; }
    public DateTime UltimaAtualizacaoClima { get; set; }
    
    // Alertas
    public int AlertasAtivos { get; set; }
    public List<AlertaDto> AlertasRecentes { get; set; } = new();
    
    // Status geral
    public bool SistemaOperacional { get; set; }
    public DateTime UltimaAtualizacao { get; set; }
}

/// <summary>
/// DTO para Alerta.
/// </summary>
public class AlertaDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public bool Resolvido { get; set; }
    public DateTime DataCriacao { get; set; }
}
