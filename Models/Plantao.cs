namespace Yrke.Models
{
    public class Plantao
    {
     
            public int Id { get; set; }

            public string UsuarioId { get; set; }

            public DateTime Data { get; set; }

            public string Turno { get; set; } // Manhã ou Noite
        
    }
}
