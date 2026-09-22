namespace BioTrilhos.API.Models;

/// <summary>
/// Armazena as configurações ambientais ideais para a estufa.
/// </summary>
public class ConfiguracoesAmbientais
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public decimal UmidadeSoloMinima { get; set; } = 50;
    public decimal UmidadeSoloMaxima { get; set; } = 75;
    public decimal TemperaturaMinima { get; set; } = 20;
    public decimal TemperaturaMaxima { get; set; } = 28;
    public decimal UmidadeArMinima { get; set; } = 60;
    public decimal UmidadeArMaxima { get; set; } = 80;
    public int TempoMaximoIrrigacaoSegundos { get; set; } = 300;
    public decimal NivelMinimoReservatorio { get; set; } = 20;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Estufa? Estufa { get; set; }
}
