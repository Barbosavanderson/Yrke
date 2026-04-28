using Microsoft.EntityFrameworkCore;
using Yrke.Data;
using Yrke.Models;
public static class SeedData
{
    public static void Inicializar(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            if (context.Plantoes.Any())
                return; // já tem dados

            context.Plantoes.AddRange(
                new Plantao
                {
                    UsuarioId = "joao",
                    Data = DateTime.Today.AddDays(1),
                    Turno = "Manhã"
                },
                new Plantao
                {
                    UsuarioId = "maria",
                    Data = DateTime.Today.AddDays(1),
                    Turno = "Noite"
                },
                new Plantao
                {
                    UsuarioId = "joao",
                    Data = DateTime.Today.AddDays(3),
                    Turno = "Noite"
                }
            );

            context.SaveChanges();
        }
    }
}