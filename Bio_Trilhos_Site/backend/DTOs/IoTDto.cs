namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para receber dados do Arduino/IoT.
/// </summary>
public class IoTLeituraDto
{
    public string DispositivoId { get; set; } = string.Empty;
    public decimal? Temperatura { get; set; }
    public decimal? UmidadeAr { get; set; }
    public decimal? UmidadeSolo { get; set; }
    public decimal? Luminosidade { get; set; }
    public decimal? NivelAgua { get; set; }
    public bool? Chuva { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// DTO para resposta de recebimento IoT.
/// </summary>
public class IoTRespostaDto
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public List<string> Erros { get; set; } = new();
}
