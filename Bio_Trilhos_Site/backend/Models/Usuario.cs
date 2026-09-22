namespace BioTrilhos.API.Models;

/// <summary>
/// Tipos de usuários do sistema.
/// </summary>
public enum TipoUsuario
{
    Admin,
    Operador,
    Visualizacao
}

/// <summary>
/// Representa um usuário do sistema.
/// </summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public TipoUsuario TipoUsuario { get; set; } = TipoUsuario.Operador;
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
