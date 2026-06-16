using System.ComponentModel.DataAnnotations;

namespace Estufa.Api.Models
{
    public class Configuracao
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Chave { get; set; } = null!;
        public string Valor { get; set; } = null!;
        public string? Descricao { get; set; }
    }
}
