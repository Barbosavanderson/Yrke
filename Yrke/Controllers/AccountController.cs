using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Yrke.Data;
using Yrke.Models;
using Yrke.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Yrke.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

       
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Buscar usuário apenas pelo email
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user != null)
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Senha, model.Senha);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Credenciais inválidas");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Cadastrar(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
           
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email já cadastrado");
                return View(model);
            }
            var newUser = new User
            {
                Nome = model.Nome,
                Email = model.Email,
                Telefone = model.Telefone,
                TipoEscala = model.TipoEscala,
             
            };
            // deixando a senha mais segura 

            var hasher = new PasswordHasher<User>();
            newUser.Senha = hasher.HashPassword(newUser, model.Senha);

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View();
        }
    }
}
