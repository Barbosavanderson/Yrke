using Microsoft.AspNetCore.Identity;
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
            var hasher = new PasswordHasher<User>();

            var admin = context.Users.FirstOrDefault(u => u.Email == "admin@yrke.com");
            if (admin == null)
            {
                admin = new User
                {
                    Nome = "Administrador",
                    Email = "admin@yrke.com",
                    Telefone = "0000000000",
                    Funcao = "Administrador",
                    TipoEscala = "Diurno",
                    Role = "Administrador",
                    UrlFoto = "/images/avatar-default.png",
                    DataCadastro = DateTime.UtcNow
                };
                admin.Senha = hasher.HashPassword(admin, "Admin@123");
                context.Users.Add(admin);
            }

            var joao = context.Users.FirstOrDefault(u => u.Email == "joao@yrke.com");
            if (joao == null)
            {
                joao = new User
                {
                    Nome = "João",
                    Email = "joao@yrke.com",
                    Telefone = "11999990000",
                    Funcao = "Enfermeiro",
                    TipoEscala = "Manhã",
                    Role = "Funcionario",
                    UrlFoto = "/images/avatar-default.png",
                    DataCadastro = DateTime.UtcNow
                };
                joao.Senha = hasher.HashPassword(joao, "Senha123!");
                context.Users.Add(joao);
            }

            var maria = context.Users.FirstOrDefault(u => u.Email == "maria@yrke.com");
            if (maria == null)
            {
                maria = new User
                {
                    Nome = "Maria",
                    Email = "maria@yrke.com",
                    Telefone = "11988880000",
                    Funcao = "Técnica",
                    TipoEscala = "Noite",
                    Role = "Funcionario",
                    UrlFoto = "/images/avatar-default.png",
                    DataCadastro = DateTime.UtcNow
                };
                maria.Senha = hasher.HashPassword(maria, "Senha123!");
                context.Users.Add(maria);
            }

            if (context.ChangeTracker.HasChanges())
                context.SaveChanges();

            if (!context.Plantoes.Any())
            {
                context.Plantoes.AddRange(
                    new Plantao
                    {
                        UsuarioId = joao.Id.ToString(),
                        Data = DateTime.Today.AddDays(1),
                        Turno = "Manhã"
                    },
                    new Plantao
                    {
                        UsuarioId = maria.Id.ToString(),
                        Data = DateTime.Today.AddDays(1),
                        Turno = "Noite"
                    },
                    new Plantao
                    {
                        UsuarioId = joao.Id.ToString(),
                        Data = DateTime.Today.AddDays(3),
                        Turno = "Noite"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
