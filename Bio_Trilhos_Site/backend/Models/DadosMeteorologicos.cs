namespace BioTrilhos.API.Models;

/// <summary>
/// Armazena dados recebidos da API meteorológica externa.
/// </summary>
public class DadosMeteorologicos
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
    public DateTime DataHoraConsulta { get; set; } = DateTime.UtcNow;
    public string FonteAPI { get; set; } = "Open-Meteo";

    // Relacionamentos
    public virtual Estufa? Estufa { get; set; }
}
