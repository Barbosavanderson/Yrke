using System.ComponentModel.DataAnnotations;

namespace Yrke.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome completo")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        [Display(Name = "Telefone")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "A função é obrigatória")]
        [Display(Name = "Função")]
        [StringLength(50, ErrorMessage = "A função deve ter no máximo 50 caracteres")]
        public string Funcao { get; set; } = string.Empty;

        [Display(Name = "Tipo de escala")]
        [StringLength(50, ErrorMessage = "O tipo de escala deve ter no máximo 50 caracteres")]
        public string TipoEscala { get; set; } = string.Empty;
    }
}