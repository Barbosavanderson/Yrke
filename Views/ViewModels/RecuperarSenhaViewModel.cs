using System.ComponentModel.DataAnnotations;

namespace Yrke.ViewModels
{
    public class RecuperarSenhaViewModel
    {
        [Required (ErrorMessage = "Email é obrigaório")]
        [EmailAddress (ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;
    }
}
