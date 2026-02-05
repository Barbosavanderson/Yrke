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
        private readonly EmailService _emailService;

        public AccountController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

                    if  (result == PasswordVerificationResult.Success)
                    
                    {
                        var userClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Nome),
                            new Claim(ClaimTypes.Email, user.Email)
                        };

                        var Identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(Identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        TempData["WelcomeMessage"] = $"Bem-Vindo ao Yrke, {user.Nome}!";
                    }
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
        [HttpPost]
        public IActionResult RecuperarSenha(RecuperarSenhaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email não encontrado");
                return View(model);
            }

            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.TokenExpiration = DateTime.Now.AddMinutes(15);

            _context.Users.Update(user);
            _context.SaveChanges();

            var resetLink = Url.Action("RedefinirSenha", "Account", new { token = token }, Request.Scheme);

            // Para testes
            _emailService.SendEmail(user.Email, "Redefinição de Senha", $"Clique no link para redefinir sua senha: {resetLink}");

            ViewBag.Message = "Um link de redefinição foi gerado. Verifique seu email.";
            return View();
        }

        [HttpGet]
        public IActionResult RedefinirSenha(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetToken == token && u.TokenExpiration > DateTime.Now);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new RedefinirSenhaViewModel { Token = token };
            return View(model);
        }
        [HttpPost]
        public IActionResult RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.ResetToken == model.Token && u.TokenExpiration > DateTime.Now);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var hasher = new PasswordHasher<User>();
            user.Senha = hasher.HashPassword(user, model.NovaSenha);

            user.ResetToken = null;
            user.TokenExpiration = null;

            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Login", "Account");
        }

    }
}