namespace BioTrilhos.API.DTOs;

/// <summary>
/// DTO para criar ou atualizar uma Estufa.
/// </summary>
public class CreateEstufaDto
{
    public string Nome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;
}

/// <summary>
/// DTO para resposta de Estufa.
/// </summary>
public class EstufaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
}
