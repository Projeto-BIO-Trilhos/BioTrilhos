namespace BioTrilhos.API.Models;

/// <summary>
/// Níveis de severidade dos alertas.
/// </summary>
public enum NivelAlerta
{
    Info,
    Aviso,
    Critico
}

/// <summary>
/// Armazena alertas e situações importantes do sistema.
/// </summary>
public class Alerta
{
    public int Id { get; set; }
    public int EstufaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public NivelAlerta Nivel { get; set; } = NivelAlerta.Aviso;
    public bool Resolvido { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataResolucao { get; set; }

    // Relacionamentos
    public virtual Estufa? Estufa { get; set; }
}
