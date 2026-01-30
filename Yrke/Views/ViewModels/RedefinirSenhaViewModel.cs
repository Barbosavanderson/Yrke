using System.ComponentModel.DataAnnotations;


namespace Yrke.ViewModels
{
    public class RedefinirSenhaViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NovaSenha { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não conferem")]
        public string ConfirmarSenha { get; set; }
    }
}
