using System;

namespace Yrke.ViewModels
{
    public class ProfileViewModel
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Funcao { get; set; } = string.Empty;
        public string TipoEscala { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }

        // Estatísticas (valores placeholder por enquanto)
        public int TotalPlantioes { get; set; } = 0;
        public int SatisfacaoMedia { get; set; } = 0;
    }
}