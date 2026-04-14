using System.ComponentModel.DataAnnotations;

namespace Yrke.ViewModels
{
        
    public class PerfilViewModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Nome é obrigatório")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "Email é obrigatório")]
            [EmailAddress(ErrorMessage = "Email inválido")]
            public string Email { get; set; }

            public string Telefone { get; set; }

            [Required(ErrorMessage = "Função é obrigatória")]
            public string Funcao { get; set; }

            [Required(ErrorMessage = "Tipo de escala é obrigatório")]
            public string TipoEscala { get; set; }

            // URL da foto salva no servidor (ex: "/uploads/perfil/abc.jpg")
            public string UrlFoto { get; set; } = "/images/avatar-default.png";

            // Propriedade para receber o arquivo do formulário (não vai pro banco, só pro servidor)
            public IFormFile FotoFile { get; set; }
        }
    }


    
