namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para criar uma Leitura de Sensor.
/// </summary>
public class CreateLeituraDto
{
    public int SensorId { get; set; }
    public decimal Valor { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public string Status { get; set; } = "OK";
}

/// <summary>
/// DTO para resposta de Leitura de Sensor.
/// </summary>
public class LeituraDto
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataHoraLeitura { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
