using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Yrke.ViewModels
{
    public class PerfilViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(120, ErrorMessage = "Nome deve ter no máximo 120 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(120, ErrorMessage = "Email deve ter no máximo 120 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(30, ErrorMessage = "Telefone deve ter no máximo 30 caracteres")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Função é obrigatória")]
        [StringLength(50, ErrorMessage = "Função deve ter no máximo 50 caracteres")]
        public string Funcao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de escala é obrigatório")]
        [StringLength(20, ErrorMessage = "Tipo de escala deve ter no máximo 20 caracteres")]
        public string TipoEscala { get; set; } = string.Empty;

        // URL da foto salva no servidor (ex: "/uploads/perfil/abc.jpg")
        public string UrlFoto { get; set; } = "/images/avatar-default.png";

        // Propriedade para receber o arquivo do formulário (não vai pro banco, só pro servidor)
        public IFormFile? FotoFile { get; set; }
    }
}
