using System.ComponentModel.DataAnnotations;

namespace Yrke.ViewModels
{ 
public class RegisterViewModel
{
    [Required]
    [StringLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(30)]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Funcao { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string TipoEscala { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 6)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Senha), ErrorMessage = "As senhas não conferem")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}
}