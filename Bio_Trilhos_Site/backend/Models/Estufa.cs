namespace BioTrilhos.API.Models;

/// <summary>
/// Representa uma estufa no sistema Bio Trilhos.
/// </summary>
public class Estufa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<Sensor> Sensores { get; set; } = new List<Sensor>();
    public virtual ICollection<Reservatorio> Reservatorios { get; set; } = new List<Reservatorio>();
    public virtual ICollection<Irrigacao> Irrigacoes { get; set; } = new List<Irrigacao>();
    public virtual ICollection<ConfiguracoesAmbientais> ConfiguracoesAmbientais { get; set; } = new List<ConfiguracoesAmbientais>();
    public virtual ICollection<DadosMeteorologicos> DadosMeteorologicos { get; set; } = new List<DadosMeteorologicos>();
    public virtual ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}
