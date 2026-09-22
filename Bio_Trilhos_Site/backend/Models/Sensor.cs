namespace BioTrilhos.API.Models;

/// <summary>
/// Tipos de sensores suportados no sistema.
/// </summary>
public enum TipoSensor
{
    Temperatura,
    UmidadeDoAr,
    UmidadeDoSolo,
    Luminosidade,
    Chuva,
    NivelDeAgua
}

/// <summary>
/// Representa um sensor conectado à estufa.
/// </summary>
public class Sensor
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoSensor TipoSensor { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public string IdentificadorDispositivo { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataInstalacao { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Estufa? Estufa { get; set; }
    public virtual ICollection<LeituraSensor> Leituras { get; set; } = new List<LeituraSensor>();
}
