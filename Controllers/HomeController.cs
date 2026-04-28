using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using Yrke.Data;
using Yrke.Models;
using Yrke.ViewModels;

namespace Yrke.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;



        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Perfil()
        {
            return View();
        }


        [Authorize(Roles = "Administrador")]
        public IActionResult DashboardAdmin()
        {
            return View();
        }

        public IActionResult Sobre()
        {
            return View();
        }

        public IActionResult Contato()
        {
            return View();
        }

        public IActionResult Investimento()
        {
            return View();
        }
        public IActionResult Trabalhos() => View();

        public IActionResult InserirTeste()
        {
            _context.Plantoes.Add(new Plantao
            {
                UsuarioId = "joao",
                Data = DateTime.Today,
                Turno = "Manhã"
            });

            _context.Plantoes.Add(new Plantao
            {
                UsuarioId = "maria",
                Data = DateTime.Today.AddDays(1),
                Turno = "Noite"
            });

            _context.SaveChanges();

            return Content("Inserido!");
        }


    }
}