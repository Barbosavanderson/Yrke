using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Yrke.Data;
using Yrke.Models;

[Authorize]
public class PlantaoController : Controller
{
    private readonly ApplicationDbContext _context;

    public PlantaoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var plantoesQuery = _context.Plantoes.AsQueryable();

        // Filtrar apenas plantões do usuário autenticado
        plantoesQuery = plantoesQuery.Where(p => p.UsuarioId == userId);

        var plantoes = plantoesQuery
            .Select(p => new
            {
                title = p.Turno,
                date = p.Data.ToString("yyyy-MM-dd"),
                color = p.Turno == "Manhã" ? "#2563eb" : "#1d4ed8"
            })
            .ToList();

        return Json(plantoes);
    }
}