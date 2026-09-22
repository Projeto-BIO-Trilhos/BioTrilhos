namespace BioTrilhos.API.Models;

/// <summary>
/// Armazena o histórico de leituras dos sensores.
/// </summary>
public class LeituraSensor
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataHoraLeitura { get; set; } = DateTime.UtcNow;
    public string UnidadeMedida { get; set; } = string.Empty;
    public string Status { get; set; } = "OK";

    // Relacionamentos
    public virtual Sensor? Sensor { get; set; }
}
