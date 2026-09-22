namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para Configurações Ambientais.
/// </summary>
public class ConfiguracoesAmbientaisDto
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public decimal UmidadeSoloMinima { get; set; }
    public decimal UmidadeSoloMaxima { get; set; }
    public decimal TemperaturaMinima { get; set; }
    public decimal TemperaturaMaxima { get; set; }
    public decimal UmidadeArMinima { get; set; }
    public decimal UmidadeArMaxima { get; set; }
    public int TempoMaximoIrrigacaoSegundos { get; set; }
    public decimal NivelMinimoReservatorio { get; set; }
    public DateTime AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para atualizar Configurações Ambientais.
/// </summary>
public class UpdateConfiguracoesAmbientaisDto
{
    public decimal? UmidadeSoloMinima { get; set; }
    public decimal? UmidadeSoloMaxima { get; set; }
    public decimal? TemperaturaMinima { get; set; }
    public decimal? TemperaturaMaxima { get; set; }
    public decimal? UmidadeArMinima { get; set; }
    public decimal? UmidadeArMaxima { get; set; }
    public int? TempoMaximoIrrigacaoSegundos { get; set; }
    public decimal? NivelMinimoReservatorio { get; set; }
}
