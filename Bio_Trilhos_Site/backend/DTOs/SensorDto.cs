namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para criar ou atualizar um Sensor.
/// </summary>
public class CreateSensorDto
{
    public int EstufaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int TipoSensor { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public string IdentificadorDispositivo { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de Sensor.
/// </summary>
public class SensorDto
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int TipoSensor { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public string IdentificadorDispositivo { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataInstalacao { get; set; }
}
