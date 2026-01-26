using Yrke.Models.Enum;

namespace Yrke.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Telefone { get; set; } = String.Empty;
        public string Função { get; set; } = String.Empty;
        public string TipoEscala { get; set; } = String.Empty; 
        public TipoDeAutenticacao TipoDeAutenticacao { get; set; }
        // Nova propriedade para login local
        public string Senha { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
