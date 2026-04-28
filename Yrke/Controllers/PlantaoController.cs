using Microsoft.AspNetCore.Mvc;
using Yrke.Data;
using Yrke.Models;

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
        var plantoes = _context.Plantoes
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