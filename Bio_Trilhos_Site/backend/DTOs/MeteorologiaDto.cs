namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para Dados Meteorológicos.
/// </summary>
public class DadosMeteorologicosDto
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public decimal? TemperaturaExterna { get; set; }
    public decimal? UmidadeExterna { get; set; }
    public string CondicaoClimatica { get; set; } = string.Empty;
    public bool EstaChovendo { get; set; }
    public decimal? ProbabilidadeChuva { get; set; }
    public string PrevisaoChuva { get; set; } = string.Empty;
    public decimal? VelocidadeVento { get; set; }
    public DateTime DataHoraConsulta { get; set; }
    public string FonteAPI { get; set; } = string.Empty;
}
